using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Mvc;
using Settings;
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
                return Ok(new { status = await db.AddPointsAllAsync(ammountToAdd) + " de persoane au primit cate " + ammountToAdd + " " + ProjectSettings.NumePuncteLoialitate });
            }

            return "Nu ati primit puncte ca a crapat";
        }
    }
}
