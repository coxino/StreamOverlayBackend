using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using StaticDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Coverlay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtensionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExtensionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("setinplay")]
        public async Task<ActionResult<Game>> SetInPlayAsync(string token, string url)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                string posibleGameName = "";
                var posibleGameNames = AllGamesDatabase.AllGames.Where(x => RemoveSpecialCharacters(url).Contains(RemoveSpecialCharacters(x.Game.Name)));

                if (posibleGameNames.Count() > 0)
                {
                    posibleGameNames = posibleGameNames.OrderByDescending(x => x.Game.Name.Length);
                    posibleGameName = posibleGameNames.FirstOrDefault().Game.Name;
                }

                else
                {
                    db.SetInplayGame(new InPlayGame()
                    {
                        Game = new Game() { Name = "New Game", Image = "logo.png", Potential = "Unknown", Provider = "Unknown", Rounds = new List<Round>(), Volatility = "Unknown" },
                    });
                    return new Game() { Name = "New Game", Image = "logo.png", Potential = "Unknown", Provider = "Unknown", Rounds = new List<Round>(), Volatility = "Unknown" };
                }

                db.SetInplayGame(posibleGameName);
                return Ok(db.GetInPlayGame().Game);
            }

            return new Game() { Name = "New Game", Image = "logo.png", Potential = "Unknown", Provider = "Unknown", Rounds = new List<Round>(), Volatility = "Unknown" };
        }

        [HttpGet("setrecord")]
        public async Task<ActionResult<string>> SetRecordAsync([FromHeader] string token, [FromQuery] double record, [FromQuery] double? bet)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.SaveRecord(new Round()
                {
                    PayAmount = record,
                    BetSize = bet ?? 10,
                    BonusHuntId = 0
                });

                var g = db.GetInPlayGame();
                db.SetInplayGame(g.Game.Name);
                return Ok($"Game Saved {g.Game.Name}");
            }

            return BadRequest("ERROR");
        }

        [HttpGet("currentgametobh")]
        public async Task<ActionResult<string>> AddToBHAsync([FromHeader] string token, [FromQuery] double bet)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                var g = db.GetInPlayGame();
                db.AddSingleGameToBH(g, bet);

                return Ok($"Bonus Hunt Update {g.Game.Name} - {bet}");
            }
            return BadRequest("ERROR");
        }

        [HttpGet("login")]
        public async Task<ActionResult<string>> LoginAsync(string token)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return Ok(new { message = "Connected" });
            }

            return BadRequest(new { message = "Invalid login" });
        }

        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToLower();
        }
    }
}