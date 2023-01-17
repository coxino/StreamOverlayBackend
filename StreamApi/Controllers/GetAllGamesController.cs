using DatabaseContext;
using DataLayer;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamApi.Controllers
{
    public class GameRequestModel : IEquatable<GameRequestModel>
    {
        public GameRequestModel()
        {
        }

        public string GameName { get; set; }
        public string UserId { get; set; }


        public override bool Equals(object obj)
        {
            return Equals(obj as GameRequestModel);
        }

        public bool Equals(GameRequestModel other)
        {
            return other != null &&
                   GameName == other.GameName &&
                   UserId == other.UserId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GameName, UserId);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class GetAllGamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GetAllGamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<InPlayGame>> GetAsync()
        {
            return Ok(new { allgames = StaticDatabase.AllGamesDatabase.AllGames });
        }

        [HttpPost("sellGame")]
        public async Task<ActionResult<string>> SellGameAsync([FromBody] GameRequestModel userModel)
        {
            return "NU SE MAI POT VINDE JOCURILE!";

        }

        [HttpPost("buyGame")]
        public async Task<ActionResult<string>> BuyGameAsync([FromBody] GameRequestModel userModel)
        {
            return Ok(new { message = "NU SE MAI POT CUMPARA JOLCURI!" });


            var db = await UserDatabase.GetGivewayDBAsync(_context);

            var viewer = await db.GetViewerAsync(userModel.UserId);

            if (viewer == null)
            {
                return Ok(new { message = "Te rog sa te autentifici!" });
            }

            if (db.IsUserOnCooldown(viewer.Id, "buyGame"))
            {
                return Ok(new { message = "Asteapta 10 secunde" });
            }

            if (viewer.UserCox < 4000)
            {
                return Ok(new { message = "Nu ai destul gold sa cumperi acest joc!" });
            }

            await db.AddPointsToOneUser(viewer.Id, -4000, false);

            var message = StaticDatabase.AllGamesDatabase.BuyGame(viewer.Name, userModel.GameName, out bool bought);

            if(bought == true)
            {
                db.AddUserOnCooldown(viewer.Id, "buyGame", 0.3f);
            }

            return Ok(new { message = message });
        }
    }
}
