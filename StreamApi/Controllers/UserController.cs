using DatabaseContext;
using JWTManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace StreamApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginAsync([FromHeader] string username,[FromHeader] string password)
        {
            if (await _context.Accounts.AnyAsync(x => x.Username == username && x.Password == password))
            {
               var user = await _context.Accounts.Where(x => x.Username == username && x.Password == password).FirstOrDefaultAsync();
                var result = JwtManager.GenerateToken(user);
                return Ok(result);//cache this and is done
            }
            else
            {
                return BadRequest("User don`t exists!");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<bool>> RegisterAsync([FromHeader] string username, [FromHeader] string password, [FromHeader] string email)
        {
            if (await _context.Accounts.AnyAsync(x => x.Username == username))
            {
                return BadRequest("User exists!");
            }
            else
            {
                Account account = new Account() {
                    Username = username,
                    Password = password,
                    Created = System.DateTime.Now,
                    Id = System.Guid.NewGuid(),
                    IsBanned = false,
                    Role = 0,
                    Subscription = System.DateTime.Now.AddDays(7),
                    Email = email
                };
                _context.Accounts.Add(account);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return Ok("Account created!");
                }
                else
                {
                    return BadRequest("User creation failed.");
                }
            }
        }
    }
}
