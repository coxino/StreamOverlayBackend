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
    public class Pacaniada : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Pacaniada(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ClasamentPacaniada Get([FromHeader] string username)

        {
            return UserDatabase.GetByUsername(username).GetClasamentPacaniada();
        }

        [HttpPost]
        public async Task<bool> SetAsync([FromHeader] string token, [FromBody] ClasamentPacaniada clasamentPacaniada)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return db.SetClasamentPacaniada(clasamentPacaniada);
            }

            return false;
        }
    }
}
