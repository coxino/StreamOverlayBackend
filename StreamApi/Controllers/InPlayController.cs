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
        public async Task<ActionResult<bool>> SetAsync([FromBody]InPlayGame inPlayGame,[FromHeader] string token)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.SetInplayGame(inPlayGame);
            }

            return true;
        }
    }
}
