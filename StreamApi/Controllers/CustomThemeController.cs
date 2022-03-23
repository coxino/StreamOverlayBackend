using DatabaseContext;
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
    public class CustomThemeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomThemeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public string Get([FromHeader] string username)
        {
            return UserDatabase.GetByUsername(username).GetCustomTheme();
        }

        [HttpPost]
        public async Task<ActionResult> SetAsync([FromHeader] string token, [FromHeader] string customTheme)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                db.SetCustomTheme(customTheme);
            }

            return Ok("Success");
        }
    }
}
