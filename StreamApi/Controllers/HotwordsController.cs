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
using Youtube_Contractor;

namespace StreamApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotwordsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HotwordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<HotWord> Get([FromHeader] string username)
        {
            username = username.Trim();

            return HotWords.GetHotWords(username);
        }

        [HttpPost("addhotword")]
        public ActionResult<string> Addhotword([FromHeader] string token, [FromHeader] string bettingOption)
        {            
            HotWords.AddWord(JwtManager.GetClaim(token, ClaimNames.Username).Trim(), bettingOption);

            return Ok(true);
        }

        [HttpPost("reset")]
        public ActionResult<string> Reset([FromHeader] string token)
        {
            HotWords.ResetHotWords(token);
            return Ok(true);
        }

        [HttpPost("final")]
        public async Task<ActionResult<string>> BalantaFinalaAsync([FromHeader] string token, [FromHeader] string userID, [FromHeader] string ammount)
        {
            var db = await UserDatabase.GetGivewayDBAsync(_context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                if (int.TryParse(ammount, out int amnt))
                {
                    var vr = await db.GetViewerAsync(userID);
                    if (GiveawayAniversar.AdaugaBalanta(userID, amnt))
                    {
                        return $"{vr.Name} ai votat ca balanta finala va fi {amnt}!";
                    }
                    else
                    {
                        return $"{vr.Name} nu poti vota balanta finala de 2 ori!";
                    }
                }
            }

            return $"";
        }

        [HttpGet("bfusers")]
        public ActionResult<int> GetBFUsers()
        {
            return GiveawayAniversar.BalantaFinala.Count;
        }

        [HttpPost("extragebf")]
        public async Task<ActionResult<string>> ExtrageBalantaFinalaAsync([FromHeader] string token,[FromHeader] string ammount)
        {
            var db = await UserDatabase.GetGivewayDBAsync(_context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                if (int.TryParse(ammount, out int amnt))
                {
                    var winner = GiveawayAniversar.CastigatorBalantaFinala(amnt);

                    var winnerName = await db.GetViewerAsync(winner);

                    await new YoutubeChatWriter().WriteMessageAsync($"Felicitari - {winnerName.Name} ai fost cel mai aproape de balanta finala cu predictia ta {GiveawayAniversar.BalantaFinala[winnerName.Id]}! ID verificare {winner}");

                    return $"Felicitari - {winnerName.Name} ai fost cel mai aproape de balanta finala cu predictia ta {GiveawayAniversar.BalantaFinala[winnerName.Id]}! ID verificare {winner}";
                }
            }

            return $"nu am putut extrage niciun castigator!";
        }

        [HttpPost("lma")]
        public async Task<ActionResult<string>> LaMultiAniAsync([FromHeader] string token, [FromHeader] string userID, [FromHeader] string userName)
        {
            var db = await UserDatabase.GetGivewayDBAsync(_context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return GiveawayAniversar.AdaugaUser(userID, userName);
            }

            return $"{userName} intampin probleme cu adaugarea ta in giveaway!";
        }

        [HttpGet("lmausers")]
        public ActionResult<int> GetLMAUsers()
        {
            return GiveawayAniversar.Users.Count;
        }

        [HttpPost("extragelma")]
        public async Task<ActionResult<string>> ExtrageLaMultiAniAsync([FromHeader] string token)
        {
            var db = await UserDatabase.GetGivewayDBAsync(_context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                var winner =  GiveawayAniversar.ExtrageUnCastigator();

                var winnerName = await db.GetViewerAsync(winner);  

                await new YoutubeChatWriter().WriteMessageAsync($"Felicitari - {winnerName.Name} ai castigat la giveaway! ID verificare {winner}");

                return $"Felicitari - {winnerName.Name} ai castigat la giveaway! ID verificare {winner}";
            }

            return $"nu am putut extrage niciun castigator!";
        }

    }
}
