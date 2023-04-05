using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Mvc;
using StaticDatabase;
using System.Threading.Tasks;

namespace StreamApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BonushuntController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BonushuntController(ApplicationDbContext context)
        {
            _context = context;
        }

        public BonusHuntFullInfo GetAsync([FromHeader] string token, [FromHeader] string username)
        {
            var bh = UserDatabase.GetByUsername(username).GetLiveBonusHunt();
           // bh.HuntInfo = UserDatabase.GetByUsername(username).getInfoFromBH(bh);
            return bh;
        }


        [HttpPost("set")]
        public async Task<ActionResult<bool>> SetAsync([FromHeader] string token, [FromBody] BonusHuntFullInfo bonusHunt)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.UpdateBonusHunt(bonusHunt);
            }

            return Ok(true);
        }

        [HttpPost("add")]
        public async Task<ActionResult<bool>> AddAsync([FromHeader] string token, [FromHeader] double betSize, [FromBody] InPlayGame gameName)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {               
                db.AddSingleGameToBH(gameName, betSize);
            }

            return Ok(true);
        }

        //[HttpGet("qualifiers")]
        //public ActionResult<object> GetCurrent()
        //{
        //    return FinalBalance.CurentUser;
        //}

        [HttpPost("delete")]
        public async Task<ActionResult<bool>> Delete([FromHeader] string token, [FromHeader] int bonus)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
               db.DeleteLiveBonus(bonus);
            }
            return Ok(true);
        }
    }
}
