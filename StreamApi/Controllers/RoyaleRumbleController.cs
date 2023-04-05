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
    public class RoyaleRumbleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoyaleRumbleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<SlotsRumbleModel> Get([FromHeader]string username)
        {
            var who = new Database(username).GetRumbleSlots();
            return new Database(username).GetRumbleSlots();
        }

        [HttpPost("AddGameToRumble")]
        public async Task<ActionResult<bool>> AddGameToRumbleAsync([FromHeader]string token,RumbleMeci rumbleMeci)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return db.AddRumbleGame(rumbleMeci);
            }

            return false;
        }

        [HttpPost("RumbleUpdate")]
        public async Task<ActionResult<bool>> CreateNewRumbleAsync([FromHeader] string token, SlotsRumbleModel rumbleModel)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return db.RumbleUpdate(rumbleModel);
            }

            return false;
        }

        [HttpPost("CreateNewRumble")]
        public async Task<ActionResult<bool>> CreateNewRumbleAsync([FromHeader] string token)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return db.ArchiveRumble();
            }

            return false;
        }
    }
}
