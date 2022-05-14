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

namespace StreamApi.Controllers
{
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
        public async Task<ActionResult<bool>> BetFromTournamentAsync([FromHeader] string token,[FromHeader] int maxBet = 100)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.CreateBettingFromTournament(maxBet);
            }
            return Ok(true);
        }

        [HttpPost("updateOption")]
        public async Task<ActionResult<string>> UpdateOptionAsync([FromHeader] string token,[FromHeader] string userID, [FromHeader] string bettingOption, [FromHeader] string user, [FromHeader] string amount)
        {
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
                return $"{userName} ai {(await db.GetCoxiCoinsAsync(userID, "", "", "", userName)).CoxiCoins} cox. Ii poti schimba in premii pe !shop.";
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

        [HttpPost("gamble")]
        public async Task<ActionResult<string>> Moca2OptionAsync([FromHeader] string token, [FromHeader] string userID, [FromHeader] string user,[FromHeader] int ammount)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                if(db.IsUserOnCooldown(userID, "gamble"))
                {
                    return $"@{user} poti paria o data la 5 minute!";
                }
               
                if(ammount > 1000 || ammount <= 0)
                {
                    return $"@{user} poti paria doar intre 1 si 1 000 cox.";
                }

                var cc = await db.GetCoxiCoinsAsync(userID, "", "", "","");
                if (cc.CoxiCoins < ammount)
                {
                    return $"@{user} ai {cc.CoxiCoins} cox nu poti paria {ammount}.";
                }

                db.AddUserOnCooldown(userID, "gamble", 5);
                bool won = new Random().Next(1, 101) < 48;
                if (won == true)
                {
                    await db.RegisterNewViewerFromChat(userID, user, ammount);
                    return $"@{user} ai pariat {ammount} si a castigat {ammount*2}! FELIKITARI";
                }
                else
                {
                    await db.RegisterNewViewerFromChat(userID, user, ammount * -1);
                    return $"@{user} ai pierdut {ammount} cox! Lasa jocurile ca nu sunt de tine!";
                }
            }
            return $"{user} mai incearca odata...";
        }

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
                var q = await db.AddPointsToOneUser(userID, ammount/10);
                return $"{userName} " + q;
            }

            return string.Format("{0} vezi ca a crapat transferul a {1} de geuri de cox", userName, ammount/10);
        }

        [HttpPost("donate")]
        public async Task<ActionResult<string>> donatecox([FromHeader] string token, [FromHeader] string senderID, [FromHeader] string userName, [FromHeader] int ammount)
        {
            userName = userName.Trim().Replace("@","");
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

                            if (sender.Inventory > ammount)
                            {
                                await db.AddPointsToOneUser(senderID, -1 * ammount,false);
                                await db.AddPointsToOneUser(viwers.Where(x => x.Name == userName).FirstOrDefault().Id, ammount,false);
                                return $"@{userName} ai primit {ammount} cox de la {sender.Name}!";
                            }
                            else
                            {
                                return $"A picat adaugarea a {ammount} cox utilizatorului @{userName} pentru ca sunt prea multi cu acelasi nume, incearca cu ID-ul!";
                            }

                        }
                        else
                        {
                            var viewer = await db.GetViewerAsync(userName);
                            if (viewer != null)
                            {
                                if (sender.Inventory > ammount)
                                {
                                    await db.AddPointsToOneUser(senderID, -1 * ammount,false);
                                    await db.AddPointsToOneUser(viwers.Where(x => x.Id == userName).FirstOrDefault().Id, ammount,false);
                                    return $"@{userName} ai primit {ammount} cox de la {sender.Name}!";
                                }
                                else
                                {
                                    return $"{sender.Name} nu poti trimite {ammount} cox, pentru ca ai doar {sender.Inventory}";
                                }
                            }
                            else
                            {
                                return $"A picat adaugarea a {ammount} cox utilizatorului @{userName} pentru ca sunt prea multi cu acelasi nume, incearca cu ID-ul!";
                            }
                        }
                    }
                    else
                    {
                        return $"{sender.Name} nu poti trimite {ammount} cox, pentru ca nu esti membru, sau nivelul tau nu permite acest lucru!";
                    }
                }                
            }

            return string.Format("{0} vezi ca a crapat transferul a {1} de geuri de cox", userName, ammount);
        }

        [HttpPost("addcox")]
        public async Task<ActionResult<string>> addcox([FromHeader] string token, [FromHeader] string userName, [FromHeader] int ammount)
        {
            userName = userName.Replace("@", "");
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                var viwers = await db.GetViewerByNameAsync(userName);
                if(viwers.Where(x=>x.Name == userName).Count() == 1){
                    var q = await db.AddPointsToOneUser(viwers.Where(x => x.Name == userName).FirstOrDefault().Id, ammount);
                    return $"@{userName} ai primit {ammount} cox!";
                }
                else
                {
                    var viewer = await db.GetViewerAsync(userName);
                    if (viewer != null)
                    {
                        var q = await db.AddPointsToOneUser(viewer.Id, ammount);
                        return $"{userName} ai primit {ammount} cox!";
                    }
                    else
                    {
                        return $"A picat adaugarea a {ammount} cox utilizatorului @{userName} pentru ca sunt prea multi cu acelasi nume!";
                    }
                }
            }

            return string.Format("{0} vezi ca a crapat transferul a {1} de geuri de cox", userName, ammount);
        }

        [HttpPost("setmember")]
        public async Task<ActionResult<string>> setmember([FromHeader] string token, [FromHeader] string member, [FromHeader] int level)
        {
            member = member.Replace("@", "");
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                var viwers = await db.GetViewerByNameAsync(member);
                if (viwers.Where(x => x.Name == member).Count() == 1)
                {
                    if (await db.SetMemberLevel(viwers.Where(x => x.Name == member).FirstOrDefault().Id, level))
                    {
                        return $"@{member} acum esti {(MemberLevels)level}!";
                    }
                    else
                    {
                        return $"A picat promovarea lui @{member} la {(MemberLevels)level} din motive necunoscute!";
                    }
                }
                else
                {
                    var viewer = await db.GetViewerAsync(member);
                    if (viewer != null)
                    {
                        if (await db.SetMemberLevel(viewer.Id, level))
                        {
                            return $"@{member} acum esti {(MemberLevels)level}!";
                        }
                        else
                        {
                            return $"A picat promovarea lui @{member} la {(MemberLevels)level} din motive necunoscute!";
                        }
                    }
                    else
                    {
                        return $"A picat adaugarea a {(MemberLevels)level} utilizatorului @{member} pentru ca sunt prea multi cu acelasi nume!";
                    }
                }
            }

            return $"A picat promovarea lui @{member} la {(MemberLevels)level} din motive necunoscute!";
        }
    }
}
