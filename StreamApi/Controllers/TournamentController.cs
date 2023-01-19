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
    public class TournamentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TournamentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<Tournament> Get([FromHeader] string userName)
        {
            userName.Replace(" ", "");
            var tourn =  UserDatabase.GetByUsername(userName).GetLiveTournament();
            return tourn;
        }

        [HttpGet("livefight")]
        public ActionResult<Meci> LiveFight([FromHeader] string userName)
        {
            userName.Replace(" ", "");
            return UserDatabase.GetByUsername(userName).GetLiveTournamentLiveFight();
        }

        [HttpPost("livefightupdate")]
        public async Task<ActionResult<bool>> LiveFightAsync([FromHeader] string token,[FromBody] Meci meci)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.UpdateTournament(meci);
            }

            return Ok();
        }

        [HttpPost("create")]
        public async Task<ActionResult<TournamentCreateInfos>> CreateTournamentAsync([FromHeader] string token, [FromBody] TournamentCreateInfos tournamenInfo)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.CreateTournament(tournamenInfo);
            }

            return tournamenInfo;
        }

        [HttpPost("update")]
        public async Task<ActionResult<bool>> updateTournamentAsync([FromHeader] string token, [FromBody] Tournament tournamenInfo)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.UpdateTournament(tournamenInfo);
            }

            return Ok(true);
        }

        [HttpPost("livefightclose")]
        public async Task<ActionResult<bool>> LiveFightcloseAsync([FromHeader] string token)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.CloseCurrentFight();
            }
            return Ok();
        }
    }
}
