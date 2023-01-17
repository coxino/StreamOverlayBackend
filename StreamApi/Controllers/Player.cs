using DatabaseContext;
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
    public class Player : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Player(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("login/{token}/{url}")]
        public async Task<string> LoginAsync(string token, string url)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                var posibleGameName = AllGamesDatabase.AllGames.Where(x => RemoveSpecialCharacters(url).Contains(RemoveSpecialCharacters(x.Game.Name))).FirstOrDefault().Game.Name;
                db.SetInplayGame(posibleGameName);
                return posibleGameName;
            }

            return "";
        }

        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }
    }
}