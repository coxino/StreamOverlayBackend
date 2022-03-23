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

        public List<BettingModel> Get([FromHeader] string username)
        {
            return UserDatabase.GetByUsername(username).GetLiveBetting();
        }

        [HttpPost("set")]
        public async Task<ActionResult<bool>> SetAsync([FromHeader] string token, [FromBody] List<BettingModel> bettingModel)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.SetLiveBetting(bettingModel);
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

        [HttpPost("prezenta")]
        public async Task<ActionResult<string>> PrezentaAsync([FromHeader] string token, [FromHeader] string userID)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return await db.SetUserPrecence(userID);
            }
            return $"nu am putut seta prezenta...";
        }

        [HttpPost("moca")]
        public async Task<ActionResult<string>> MocaOptionAsync([FromHeader] string token, [FromHeader] string userID, [FromHeader] string user)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
               return await db.AddPointsAsync(userID, user, 25, "moca");
            }
            return $"{user} mai incearca odata...";
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
                    return $"@{user} poti paria doar intre 1 si 1 000 fise.";
                }

                var cc = await db.GetCoxiCoinsAsync(userID, "", "", "");
                if (cc.CoxiCoins < ammount)
                {
                    return $"@{user} ai {cc.CoxiCoins} fise nu poti paria {ammount}.";
                }

                db.AddUserOnCooldown(userID, "gamble", 5);
                bool won = new Random().Next(1, 101) < 48;
                if (won == true)
                {
                    await db.AddPointsAsync(userID, user, ammount);
                    return $"@{user} ai pariat {ammount} si a castigat {ammount*2}! FELIKITARI";
                }
                else
                {
                    await db.AddPointsAsync(userID, user, ammount * -1);
                    return $"@{user} ai pierdut {ammount} fise! Lasa jocurile ca nu sunt de tine!";
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
                return await db.AddPointsAsync(userID, userName, ammount/100);
            }

            return string.Format("{0} vezi ca a crapat transferul a {1} de geuri de cox", userName, ammount/100);
        }
    }
}
