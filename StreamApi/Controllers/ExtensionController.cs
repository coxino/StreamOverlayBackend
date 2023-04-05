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

namespace StreamApi.Controllers
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
                var posibleGameName = AllGamesDatabase.AllGames.Where(x => RemoveSpecialCharacters(url).Contains(RemoveSpecialCharacters(x.Game.Name))).FirstOrDefault()?.Game?.Name;
                if (string.IsNullOrWhiteSpace(posibleGameName))
                {
                    return new Game() { Name = "New Game", Image = "logo.png", Potential = "Unknown", Provider = "Unknown", Rounds = new List<Round>(), Volatility = "Unknown" };
                }
                db.SetInplayGame(posibleGameName);
                return Ok(db.GetInPlayGame().Game);
            }

            return new Game() { Name = "New Game", Image="logo.png",Potential="Unknown", Provider="Unknown", Rounds = new List<Round>(),Volatility = "Unknown" };
        }

        [HttpGet("login")]
        public async Task<ActionResult<string>> LoginAsync(string token)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return Ok(new { message = "Connected" });
            }

            return BadRequest(new {message="Invalid login"});
        }

        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToLower();
        }
    }
}