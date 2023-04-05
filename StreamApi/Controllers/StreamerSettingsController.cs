using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Youtube_Contractor;
using static Google.Apis.Requests.BatchRequest;

namespace StreamApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamerSettingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StreamerSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getredeems")]
        public async Task<ActionResult<List<object>>> GetRedeemsAsync([FromQuery] string token)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                var rdm = db.GetAllRedeems();
                if(rdm != null)
                foreach(var redeem in rdm)
                {
                    redeem.ViewerSettings = db.GetUserSettingsForStreamerPage(redeem.UserId);                    
                }

                return Ok(new { redeems = rdm, allViewers = await db.GetAllViewers(), ytmembers = db.GetStreamerYoutubeSubscribers(), twitchmembers = db.GetStreamerTwitchSubscribers() });
            }

            return Ok("Error");
        }

        [HttpGet("setyoutubetoken")]
        public async Task<ActionResult<bool>> SetStreamerYoutubeTokenAsync([FromQuery] string token, [FromQuery] string youtubetoken)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                if(!string.IsNullOrEmpty(youtubetoken))
                {
                    var response = ViewerAPIRequest.GetMemberLevel(youtubetoken);
                }
                return Ok(new { status = await db.SetYoutubeToken(youtubetoken), count = "YT MEMBERS NOT IMPLEMENTED... YET" });
            }

            return Ok(new { status = false });
        }

        [HttpGet("settwitchtoken")]
        public async Task<ActionResult<bool>> SetStreamerTwitchTokenAsync([FromQuery] string token, [FromQuery] string twitchtoken, [FromQuery] string twitchid)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                await db.SetTwitchToken(twitchtoken, twitchid);                

                List<TwitchSubscription> response = await ViewerAPIRequest.GetTwitchMemberLevelAsync(twitchid, twitchtoken);

                return Ok(new { status = db.SaveStreamerTwitchSubscribers(response), count = response.Count });
            }
            
            return Ok(new { status = false });
        }

        [HttpGet("getstreamersettings")]
        public async Task<ActionResult<StreamerSettings>> GetStreamerSettingsAsync([FromQuery] string token)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return Ok(new { streamerSettings = db.GetStreamerSettings(), memberLevelsMap = db.GetMemberLevelMap() });
            }

            return Ok(new { streamerSettings = new StreamerSettings(), memberLevelsMap = "null" });  
        }

        [HttpPost("savestreamersettings")]
        public async Task<ActionResult<bool>> SaveStreamerSettingsAsync([FromQuery] string token, [FromBody] StreamerSettings settings)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return db.SaveStreamerSettings(settings);
            }

            return Ok(false);
        }

        [HttpGet("getstreamerviewerformsetting")]
        public ActionResult<List<RequestFromViewerForm>> GetStreamerViewerFormSettingSet([FromQuery] string localUserToken, [FromQuery] string streamerId)
        {
            _ = JwtManager.GetClaim(localUserToken, ClaimNames.Username);

            var db = UserDatabase.GetByUsername(streamerId);

            var StreamerSettings = db.GetStreamerSettings();

            return Ok(new { streamerSettings = StreamerSettings });
        }

        [HttpPost("saveusersettingsforstreamerpage")]
        public async Task<ActionResult<bool>> SaveUserSettingsForStreamerPageSetAsync([FromQuery] string localUserToken, [FromQuery] string streamerid, [FromBody] List<RequestFromViewerForm> viewerForm)
        {
            var viewerId = JwtManager.GetClaim(localUserToken, ClaimNames.Username);
            var db = await UserDatabase.GetGivewayDBAsync(_context, streamerid);

            return Ok(new { form = db.SaveUserSettingsForStreamerPage(viewerForm, viewerId) });
        }

        [HttpPost("savemembermap")]
        public async Task<ActionResult<bool>> SaveMemberMapAsync([FromQuery] string token, [FromBody] List<YoutubeMemberLevelMap> levelMaps)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return db.SaveMembersMap(levelMaps);
            }

            return Ok(false);
        }

        [HttpGet("getusersettingsforstreamerpage")]
        public async Task<ActionResult<List<RequestFromViewerForm>>> GetUserSettingsForStreamerPageSetAsync([FromQuery] string localUserToken, [FromQuery] string streamerid)
        {
            var viewerId = JwtManager.GetClaim(localUserToken, ClaimNames.Username);
            var db = await UserDatabase.GetGivewayDBAsync(_context, streamerid);

            return Ok(new { form = db.GetUserSettingsForStreamerPage(viewerId) });
        }
    }
}
