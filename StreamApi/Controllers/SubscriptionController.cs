using DatabaseContext;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace StreamApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SubscriptionModel> GetAsync([FromHeader] string username)
        {
            Account userAccount =  await _context.Accounts.Where(x => x.Username == username).FirstOrDefaultAsync();

            return new SubscriptionModel() { EndsOn = userAccount.Subscription };
        }
    }
}
