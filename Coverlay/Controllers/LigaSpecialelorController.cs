using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coverlay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LigaSpecialelorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LigaSpecialelorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("clasament")]
        public ActionResult<List<LigaUser>> Get([FromHeader] string username)
        {
            return Ok(new
            {
                Users = UserDatabase.GetByUsername(username).GetLiga()
            });
        }

        [HttpPost("updateuser")]
        public async Task<ActionResult<bool>> UpdateUserAsync([FromHeader] string token, [FromBody] LigaUser user)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return db.AddLiga(user);
            }
            return false;
        }

        [HttpPost("sterge")]
        public async Task<ActionResult<bool>> StergeUserAsync([FromHeader] string token, [FromBody] LigaUser user)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return db.StergeUserDinLiga(user);
            }
            return false;
        }

    }
}
