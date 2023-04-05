using LocalDatabaseManager;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DataLayer;
using DatabaseContext;
using JWTManager;
using System.Collections.Generic;

namespace Coverlay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoyalityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoyalityController(ApplicationDbContext context)
        {
            _context = context;
        }

        //e used pe overlaywebsite
        [HttpGet("ranking")]
        public async Task<ActionResult<List<Viewer>>> GetAsync([FromHeader] string token)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);

            if(db.GetAccountID() != "7963ff08-88e6-4ce5-8b4f-f7c0bcb9e783")
            {
                return BadRequest();
            }

            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
               return await db.GetLoyaltyPointsAsync();
            }
            return null;
        }       
    }
}
