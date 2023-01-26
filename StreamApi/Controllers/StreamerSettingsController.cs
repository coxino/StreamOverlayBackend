using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [HttpGet("setyoutubetoken")]
        public async Task<ActionResult<bool>> SetStreamerYoutubeTokenAsync([FromQuery] string token, [FromQuery] string youtubetoken)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return await db.SetYoutubeToken(youtubetoken);
            }

            return Ok(false);
        }

        [HttpGet("setviewersettingsset")]
        public async Task<ActionResult<bool>> SetViewerSettingsSetAsync([FromQuery] string token, [FromBody] List<RequestFromViewerForm> viewerForm)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return db.SetViewerSettings(viewerForm);
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

        [HttpGet("getusersettingsforstreamerpage")]
        public async Task<ActionResult<List<RequestFromViewerForm>>> GetUserSettingsForStreamerPageSetAsync([FromQuery] string localUserToken, [FromQuery] string streamerid)
        {
            var viewerId = JwtManager.GetClaim(localUserToken, ClaimNames.Username);
            var db = await UserDatabase.GetGivewayDBAsync(_context, streamerid);

            return Ok(new { form = db.GetUserSettingsForStreamerPage(viewerId) });
        }
    }
}
