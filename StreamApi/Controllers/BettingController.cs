using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dynamitey;
using Settings;

namespace StreamApi.Controllers
{
    public enum EventType
    {
        subscriber
    }

    public class BettingOptionOTI
    {
        public string User = "";
        public string BettingOption = "";
    }

    [Route("api/[controller]")]
    [ApiController]
    public class BettingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BettingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public BettingModel Get([FromHeader] string username)
        {
            return UserDatabase.GetByUsername(username).GetLiveBetting();
        }

        [HttpPost("set")]
        public async Task<ActionResult<bool>> SetAsync([FromHeader] string token, [FromBody] BettingModel bettingModel)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.SetLiveBetting(bettingModel);
            }
            return Ok(true);
        }

        [HttpPost("replace")]
        public async Task<ActionResult<bool>> BetFromBonushuntAsync([FromHeader] string token, [FromBody] BettingModel bettingModel)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.UpdateBetting(bettingModel);
            }
            return Ok(true);
        }

        [HttpPost("createFromBonushunt")]
        public async Task<ActionResult<bool>> BetFromBonushuntAsync([FromHeader] string token, [FromHeader] int maxBet = 100)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.CreateBettingFromBonusHunt(maxBet);
            }
            return Ok(true);
        }

        [HttpPost("createFromTournament")]
        public async Task<ActionResult<bool>> BetFromTournamentAsync([FromHeader] string token, [FromHeader] int maxBet = 100)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.CreateBettingFromTournament(maxBet);
            }
            return Ok(true);
        }

        [HttpPost("updateOption")]
        public async Task<ActionResult<string>> UpdateOptionAsync([FromHeader] string token, [FromHeader] string userID, [FromHeader] string bettingOption, [FromHeader] string user, [FromHeader] string amount)
        {
            if(Settings.ProjectSettings.BettingOff == false)
            {
                return $"@{user} Momentan nu este nicun bet activ!";
            }

            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return await db.IncrementBettingOptionAsync(userID, bettingOption, user, amount);
            }
            else
            {
                return $"@{user} mai incearca odata.. cred ca a crapat!";
            }
        }

        [HttpPost("cox")]
        public async Task<ActionResult<string>> CoxAsync([FromHeader] string token, [FromHeader] string userID, [FromHeader] string userName)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                var q = await db.GetViewerAsync(userID);
                return $"{q.Name} ai {q.UserCox} {ProjectSettings.NumePuncteLoialitate}. Ii poti schimba in premii pe !shop.";
            }
            return $"nu am putut seta prezenta...";
        }


        [HttpPost("prezenta")]
        public async Task<ActionResult<string>> PrezentaAsync([FromHeader] string token, [FromHeader] string userID, [FromHeader] string userName)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return await db.SetUserPrecence(userID, userName);
            }
            return $"nu am putut seta prezenta...";
        }

        [HttpPost("moca")]
        public async Task<ActionResult<string>> MocaOptionAsync([FromHeader] string token, [FromHeader] string userID, [FromHeader] string userName)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return await db.RegisterNewViewerFromChat(userID, userName, 25, "moca");
            }
            return $"{userName} mai incearca odata...";
        }

        //[HttpPost("gamble")]
        //public async Task<ActionResult<string>> Moca2OptionAsync([FromHeader] string token, [FromHeader] string userID, [FromHeader] string user, [FromHeader] int ammount)
        //{
        //    var db = await UserDatabase.GetDatabaseAsync(token, _context);
        //    if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
        //    {
        //        if (db.IsUserOnCooldown(userID, "gamble"))
        //        {
        //            return $"@{user} poti paria o data la 5 minute!";
        //        }

        //        if (ammount > 1000 || ammount <= 0)
        //        {
        //            return $"@{user} poti paria doar intre 1 si 1 000 {ProjectSettings.NumePuncteLoialitate}.";
        //        }

        //        var cc = await db.GetCoxiCoinsAsync(userID, "", "", "", "");
        //        if (cc.CoxiCoins < ammount)
        //        {
        //            return $"@{user} ai doar {cc.CoxiCoins} {ProjectSettings.NumePuncteLoialitate} nu poti paria {ammount}.";
        //        }

        //        db.AddUserOnCooldown(userID, "gamble", 5);
        //        bool won = new Random().Next(1, 101) < 48;
        //        if (won == true)
        //        {
        //            await db.RegisterNewViewerFromChat(userID, user, ammount);
        //            return $"@{user} ai pariat {ammount} si a castigat {ammount * 2}! FELIKITARI";
        //        }
        //        else
        //        {
        //            await db.RegisterNewViewerFromChat(userID, user, ammount * -1);
        //            return $"@{user} ai pierdut {ammount} {ProjectSettings.NumePuncteLoialitate}! Lasa jocurile ca nu sunt de tine!";
        //        }
        //    }
        //    return $"{user} mai incearca odata...";
        //}

        [HttpPost("setwinner")]
        public async Task<ActionResult<string>> SetWinner([FromHeader] string token, [FromHeader] string bettingOption)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return await db.SetBettingWinnerAsync(bettingOption);
            }

            return "A crapat in plm...";
        }

        [HttpPost("transfer")]
        public async Task<ActionResult<string>> TransferaPuncte([FromHeader] string token, [FromHeader] string userID, [FromHeader] string userName, [FromHeader] int ammount)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                var q = await db.AddPointsToOneUser(userID, ammount);
                return $"{userName} " + q;
            }

            return string.Format("{0} vezi ca a crapat transferul a {1} {2}", userName, ammount / 10, ProjectSettings.NumePuncteLoialitate);
        }

        [HttpPost("donate")]
        public async Task<ActionResult<string>> donatecox([FromHeader] string token, [FromHeader] string senderID, [FromHeader] string userName, [FromHeader] int ammount)
        {
            userName = userName.Trim().Replace("@", "");
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                var sender = await db.GetViewerAsync(senderID);
                if (sender == null)
                {
                    return $"ERROR";
                }
                else
                {
                    if (sender.MemberLevel >= MemberLevels.Ajutor)
                    {
                        var viwers = await db.GetViewerByNameAsync(userName);
                        if (viwers.Where(x => x.Name == userName).Count() == 1)
                        {

                            if (sender.UserCox > ammount)
                            {
                                await db.AddPointsToOneUser(senderID, -1 * ammount, false);
                                await db.AddPointsToOneUser(viwers.Where(x => x.Name == userName).FirstOrDefault().Id, ammount, false);
                                return $"@{userName} ai primit {ammount} {ProjectSettings.NumePuncteLoialitate} de la {sender.Name}!";
                            }
                            else
                            {
                                return $"A picat adaugarea a {ammount} {ProjectSettings.NumePuncteLoialitate} utilizatorului @{userName} pentru ca sunt prea multi cu acelasi nume, incearca cu ID-ul!";
                            }

                        }
                        else
                        {
                            var viewer = await db.GetViewerAsync(userName);
                            if (viewer != null)
                            {
                                if (sender.UserCox > ammount)
                                {
                                    await db.AddPointsToOneUser(senderID, -1 * ammount, false);
                                    await db.AddPointsToOneUser(viwers.Where(x => x.Id == userName).FirstOrDefault().Id, ammount, false);
                                    return $"@{userName} ai primit {ammount} {ProjectSettings.NumePuncteLoialitate} de la {sender.Name}!";
                                }
                                else
                                {
                                    return $"{sender.Name} nu poti trimite {ammount} {ProjectSettings.NumePuncteLoialitate}, pentru ca ai doar {sender.UserCox}";
                                }
                            }
                            else
                            {
                                return $"A picat adaugarea a {ammount} {ProjectSettings.NumePuncteLoialitate} utilizatorului @{userName} pentru ca sunt prea multi cu acelasi nume, incearca cu ID-ul!";
                            }
                        }
                    }
                    else
                    {
                        return $"{sender.Name} nu poti trimite {ammount} {ProjectSettings.NumePuncteLoialitate}, pentru ca nu esti membru, sau nivelul tau nu permite acest lucru!";
                    }
                }
            }

            return string.Format("{0} vezi ca a crapat transferul a {1} {2}", userName, ammount, ProjectSettings.NumePuncteLoialitate);
        }

        [HttpPost("addcox")]
        public async Task<ActionResult<string>> addcox([FromHeader] string token, [FromHeader] string userName, [FromHeader] int ammount)
        {
            userName = userName.Replace("@", "");
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                var viwers = await db.GetViewerByNameAsync(userName);
                if (viwers.Where(x => x.Name == userName).Count() == 1)
                {
                    var q = await db.AddPointsToOneUser(viwers.Where(x => x.Name == userName).FirstOrDefault().Id, ammount,false);
                    return $"@{userName} ai primit {ammount} {ProjectSettings.NumePuncteLoialitate}!";
                }
                else
                {
                    var viewer = await db.GetViewerAsync(userName);
                    if (viewer != null)
                    {
                        var q = await db.AddPointsToOneUser(viewer.Id, ammount, false);
                        return $"{userName} ai primit {ammount} {ProjectSettings.NumePuncteLoialitate}!";
                    }
                    else
                    {
                        return $"A picat adaugarea a {ammount} {ProjectSettings.NumePuncteLoialitate} utilizatorului @{userName} pentru ca sunt prea multi cu acelasi nume!";
                    }
                }
            }

            return string.Format("{0} vezi ca a crapat transferul a {1} {2}", userName, ammount, ProjectSettings.NumePuncteLoialitate);
        }

        [HttpPost("twitchEvents")]
        public async Task<ActionResult<string>> TwitchEvents([FromHeader] string token, [FromHeader] string eventModelJSON)
        {
            System.IO.File.AppendAllText("c:/logare/twitchEvents.txt", "\r\n" + DateTime.Now + "\r\n" + eventModelJSON);
            return Ok("Saved");
        }


            /// <summary>
            /// Logarea evenimentelor din yutube
            /// </summary>
            /// <param name="token"></param>
            /// <param name="eventModel"></param>
            /// <param name="level"></param>
            /// 
            /// 
            /// <returns></returns>
            /// 



        [HttpPost("youtubeEvents")]
        public async Task<ActionResult<string>> YoutubeEvents([FromHeader] string token, [FromHeader] string eventModelJSON)
        {
            if (eventModelJSON.Contains("\"kind\":\"youtube#liveChatMessage"))
                return "";

            System.IO.File.AppendAllText("c:/logare/youtubeEvents.txt", "\r\n" + DateTime.Now + "\r\n" + eventModelJSON);

            if (eventModelJSON.Contains("\"listener\":\"superchat-latest\""))
            {
                try
                {
                    object eventObject = Newtonsoft.Json.JsonConvert.DeserializeObject(eventModelJSON);
                    var data = Dynamic.InvokeGet(eventObject, "event");
                    var ammount = Dynamic.InvokeGet(data, "amount");
                    var channel = Dynamic.InvokeGet(eventObject, "authorDetails");
                    var channelId = Dynamic.InvokeGet(channel, "channelId");

                    var db = await UserDatabase.GetDatabaseAsync(token, _context);
                    if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
                    {
                        await Startup.YoutubeChatWriter.WriteMessageAsync(await db.AddPointsToOneUser(channelId, ammount, true));
                    }
                }
                catch
                {
                    System.IO.File.AppendAllText("c:/logare/ytDono.txt", "\r\n" + DateTime.Now + "\r\n" + eventModelJSON);
                }
            }


            if (eventModelJSON.Contains("\"type\":\"sponsor\"") && eventModelJSON.Contains("\"data\":{\"username\":"))
            {
                object eventObject = Newtonsoft.Json.JsonConvert.DeserializeObject(eventModelJSON);
                var data = Dynamic.InvokeGet(eventObject, "data");

                var id = Dynamic.InvokeGet(data, "providerId");
                string userName = Dynamic.InvokeGet(data, "username");
                string level = Dynamic.InvokeGet(data, "tier");
                DateTime expires = Dynamic.InvokeGet(eventObject, "expiresAt");

                var db = await UserDatabase.GetDatabaseAsync(token, _context);
                if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
                {
                    var viwers = await db.GetViewerByNameAsync(userName);
                    if (viwers.Where(x => x.Name == userName || x.Id == userName).Count() == 1)
                    {
                        if (await db.SetMemberLevel(viwers.Where(x => x.Name == userName || x.Id == userName).FirstOrDefault().Id, Enum.Parse<MemberLevels>(level), expires))
                        {
                            System.IO.File.AppendAllText("c:/logareYT/membrii.txt", $"[{DateTime.Now}] == {id} - {userName} - {level} - {expires} ");
                            await Startup.YoutubeChatWriter.WriteMessageAsync($"@{userName} acum esti {level}! Multumesc pentru sustinere :_coxPharoh: !");
                        }
                        else
                        {
                            System.IO.File.AppendAllText("c:/logareYT/failed.txt", $"[{DateTime.Now}] == {id} - {userName} - {level} - {expires} ");
                            await Startup.YoutubeChatWriter.WriteMessageAsync($"@{userName} din pacate nu a mers sa te promovez la {level}, vorbeste cu un admin.");
                        }
                    }
                }
            }

            return "DONE";
        }
    }
}
