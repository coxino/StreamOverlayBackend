using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return UserDatabase.GetByUsername(username).GetHotWords();
        }

        [HttpPost("addhotword")]
        public async Task<ActionResult<string>> AddhotwordAsync([FromHeader] string token, [FromHeader] string bettingOption)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return db.AddHotWord(bettingOption);
            }

            return Ok(true);
        }

        [HttpPost("reset")]
        public async Task<ActionResult<string>> ResetAsync([FromHeader] string token)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.ResetHotWors();
            }

            return Ok(true);
        }
    }
}
