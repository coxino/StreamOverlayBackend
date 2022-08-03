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
    public class DepositModel
    {
        public UserUpdateModel userUpdateModel { get; set; }
        public string deposit_id { get; set;}
        public string ammount { get; set; }
    }

    public class RegisterModel
    {
        public UserUpdateModel userUpdateModel { get; set; }
        public string registration_id { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class PostbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("deposit")]
        public async Task<ActionResult<string>> Deposit([FromBody] DepositModel userModel)
        {
            return "ai primit cox";
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterModel registerModel)
        {
            var db = await UserDatabase.GetDatabaseAsync(registerModel.userUpdateModel.token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return await db.RegisterNewViewerFromChat(registerModel.userUpdateModel.userID, "coxino", 25);
            }

            return "Din pacate nu am putut credita coxul :/";
        }
    }
}
