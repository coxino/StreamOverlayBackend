using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Mvc;
using Settings;
using System.Threading.Tasks;
using Youtube_Contractor;

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
                var msg = await db.AddPointsAllAsync(ammountToAdd) + " live viewers, recieved " + ammountToAdd + " " + ProjectSettings.NumePuncteLoialitate;
                await new YoutubeChatWriter().WriteMessageAsync(msg);
                return Ok(new { status = msg });
            }

            return "Nu ati primit puncte ca a crapat";
        }
    }
}
