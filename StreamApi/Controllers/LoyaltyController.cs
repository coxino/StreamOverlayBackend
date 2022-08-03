using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StreamApi.Controllers
{   

    [Route("api/[controller]")]
    [ApiController]
    public class LoyaltyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoyaltyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("userCoins")]
        public async Task<CoxiUser> GetAsync([FromBody] UserUpdateModel userData)
        {
            var db = await UserDatabase.GetGivewayDBAsync(_context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return await db.GetCoxiCoinsAsync(userData.userID, userData.email, userData.ipadress,userData.numeSuperbet,userData.username);
            }
            else return new CoxiUser() { CoxiCoins = 0, NumeSuperbet = "" };
        }

        [HttpPost("updateuser")]
        public async Task<ActionResult<string>> UpdateUserAsync([FromHeader] string token, [FromBody] Viewer userLoyal)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return await db.SQLContextManager.SaveUser(userLoyal) ? "success" : "failed";
            }
            return "failed";
        }

        [HttpPost("addpointsall")]
        public async Task<ActionResult<string>> AddPointsAllAsync([FromHeader] string token, [FromBody] int ammountToAdd)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return Ok(new { status = await db.AddPointsAllAsync(ammountToAdd) + " de coxati au primit cate " + ammountToAdd + " cocsi"});
            }

            return "Nu ati primit puncte ca a crapat";
        }
    }
}
