using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Youtube_Contractor;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace Coverlay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppletsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppletsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("updateyoutubemembers")]
        public async Task<ActionResult<string>> UpdateYouTubeMembers([FromHeader] string token, [FromBody] object reciever)
        {
            try
            {
                await new YoutubeChatWriter().WriteMessageAsync($"New member or rejoined!!!");
                System.IO.File.AppendAllText(@"C:\Users\COSMIN\Desktop\newmmb.txt", $"[{DateTime.Now}] Am detectat un nou membru {JsonConvert.SerializeObject(reciever)} \r\n\r\n");
            }
            catch
            {
                System.IO.File.AppendAllText(@"C:\Users\COSMIN\Desktop\newmmb.txt", $"Erroare membru nou!!!!! [{DateTime.Now}] \r\n\r\n");
            }
            var db = await UserDatabase.GetDatabaseAsync(token,_context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                YTMember YTMember = new YTMember();
                dynamic declare = JObject.Parse(reciever.ToString());
                string declare2 = JsonConvert.SerializeObject(declare);
                try
                {
                    //TESTING
                    string format = "MMMM d, yyyy 'at' h:mmtt";
                    YTMember = JsonConvert.DeserializeObject<YTMember>(declare2, new JsonSerializerSettings() { Culture = CultureInfo.InvariantCulture, DateFormatString = format });
                    int startIndex = YTMember.MemberId.IndexOf("channel/") + "channel/".Length;
                    YTMember.MemberId = YTMember.MemberId.Substring(startIndex);

                    var subs = db.GetStreamerYoutubeSubscribers() ?? new List<YTMember>();
                    if(subs.Any(x=>x.MemberId == YTMember.MemberId))
                    {
                        subs.FirstOrDefault(x => x.MemberId == YTMember.MemberId).MemberSince = YTMember.MemberSince;
                        await db.SetMemberLevel(YTMember.MemberId, YTMember.MemberTier, YTMember.MemberSince);
                        if (db.SaveStreamerYoutubeSubscribers(subs))
                        {
                            await new YoutubeChatWriter().WriteMessageAsync($"Am detectat un nou membru {YTMember.MemberName} si l-am bagat in baza de date.");
                            System.IO.File.AppendAllText(@"C:\Users\COSMIN\Desktop\newmmb.txt", $"[{DateTime.Now}] Am detectat un nou membru {YTMember.MemberName} \r\n\r\n");
                        }
                    }
                    else
                    {
                        subs.Add(YTMember);
                        await db.SetMemberLevel(YTMember.MemberId, YTMember.MemberTier, YTMember.MemberSince);
                        if (db.SaveStreamerYoutubeSubscribers(subs))
                        {
                            await new YoutubeChatWriter().WriteMessageAsync($"Am detectat un nou membru {YTMember.MemberName} si l-am bagat in baza de date.");
                            System.IO.File.AppendAllText(@"C:\Users\COSMIN\Desktop\newmmb.txt", $"[{DateTime.Now}] Am detectat un nou membru {YTMember.MemberName} \r\n\r\n");
                        }
                    }
                }
                catch (Exception ex)
                {
                    var q = ex.Message;
                    await new YoutubeChatWriter().WriteMessageAsync($"Am detectat un nou membru {declare2} din pacate nu l-am putut adauga in Baza de DATE.");
                    System.IO.File.AppendAllText(@"C:\Users\COSMIN\Desktop\errs.txt", $"[{DateTime.Now}] Am detectat un nou membru din pacate nu l-am putut adauga in Baza de DATE.\r\n DATA: {ex.Message} \r\n\r\n JSON {declare2} \r\n\r\n");
                }
               
            }
            return Ok("Loaded");
        }
    }
}
