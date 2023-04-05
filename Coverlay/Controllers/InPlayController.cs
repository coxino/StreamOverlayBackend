using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StaticDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coverlay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InPlayController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InPlayController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public InPlayGame Get([FromHeader] string username)
        
        {
            return UserDatabase.GetByUsername(username).GetInPlayGame();
        }

        [HttpPost]
        public async Task<ActionResult<bool>> SetAsync([FromBody] InPlayGame inPlayGame, [FromHeader] string token)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.SetInplayGame(inPlayGame);
            }

            return true;
        }

        [HttpPost("calificaJoc")]
        public async Task<ActionResult<bool>> SetAsync([FromBody] CupaRomanieGame game, [FromHeader] string token)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                var joc = AllGamesDatabase.AllGames.Where(x => x.Game.Name == game.Game.Name && x.PlayerName == game.PlayerName).FirstOrDefault();
                AllGamesDatabase.AllGames.Remove(joc);
                AllGamesDatabase.AllGames.Add(game);
                AllGamesDatabase.SaveGames();

                var bh = db.GetLiveBonusHunt();
                if(bh.Bonuses.Any(x=>x.GameName == game.Game.Name && x.PlayerName == game.PlayerName))
                {
                    bh.Bonuses.FirstOrDefault(x => x.GameName == game.Game.Name && x.PlayerName == game.PlayerName).Payed = game.PayOut;
                    bh.Bonuses.FirstOrDefault(x => x.GameName == game.Game.Name && x.PlayerName == game.PlayerName).BetSize = game.Bet;
                }

                db.UpdateBonusHunt(bh);
            }

            return true;
        }
    }
}
