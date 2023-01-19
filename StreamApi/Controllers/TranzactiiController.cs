using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StreamApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranzactiiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TranzactiiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public Tranzactii Get([FromHeader] string username)
        {
            return UserDatabase.GetByUsername(username).GetTranzactii();
        }

        [HttpPost]
        public async Task<ActionResult<bool>> SetAsync([FromHeader] string token, [FromBody] Tranzactii tranzactii)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                 db.SetTranzactii(tranzactii);
            }

            return Ok(true);
        }
    }
}
