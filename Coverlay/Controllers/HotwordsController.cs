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

namespace Coverlay.Controllers
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

        [HttpGet]
        public List<HotWord> Get([FromHeader] string username)
        {
            username = username.Trim();

            return HotWords.GetHotWords(username);
        }

        [HttpPost("addhotword")]
        public ActionResult<string> Addhotword([FromHeader] string token, [FromHeader] string hotword)
        {            
            HotWords.AddWord(JwtManager.GetClaim(token, ClaimNames.Username).Trim(), hotword);

            return Ok(true);
        }

        [HttpPost("reset")]
        public ActionResult<string> Reset([FromHeader] string token)
        {
            HotWords.ResetHotWords(token);
            return Ok(true);
        }

        //[HttpGet("bfusers")]
        //public ActionResult<int> GetBFUsers()
        //{
        //   // return FinalBalance.BalantaFinala.Count;
        //}

        

        //[HttpPost("lma")]
        //public async Task<ActionResult<string>> LaMultiAniAsync([FromHeader] string token, [FromHeader] string userID, [FromHeader] string userName)
        //{
        //    var db = await UserDatabase.GetGivewayDBAsync(_context);
        //    if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
        //    {
        //        return FinalBalance.AdaugaUser(userID, userName);
        //    }

        //    return $"{userName} I'm having problems with your participation in the giveaway!";
        //}

        //[HttpGet("lmausers")]
        //public ActionResult<int> GetLMAUsers()
        //{
        //    return FinalBalance.Users.Count;
        //}

        //[HttpPost("extragelma")]
        //public async Task<ActionResult<string>> ExtrageLaMultiAniAsync([FromHeader] string token)
        //{
        //    var db = await UserDatabase.GetGivewayDBAsync(_context);
        //    if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
        //    {
        //        var winner =  FinalBalance.ExtrageUnCastigator();

        //        var winnerName = await db.GetViewerAsync(winner);  

        //        await new YoutubeChatWriter().WriteMessageAsync($"Felicitari - {winnerName.Name} ai castigat la giveaway! ID verificare {winner}");

        //        return $"Felicitari - {winnerName.Name} ai castigat la giveaway! ID verificare {winner}";
        //    }

        //    return $"nu am putut extrage niciun castigator!";
        //}

    }
}
