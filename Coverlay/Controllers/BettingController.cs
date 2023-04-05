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

namespace Coverlay.Controllers
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

        [HttpGet]
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
    }
}
