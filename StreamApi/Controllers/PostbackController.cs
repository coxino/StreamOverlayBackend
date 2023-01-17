using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Settings;
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

        [HttpGet("register")]
        public ActionResult<string> Register([FromQuery] string s2s, [FromQuery] string registration_id)
        {
            System.IO.File.AppendAllText(@"C:\POSTBACK\REGS.txt", "\r\n[" + DateTime.Now + "] registration_id = " + registration_id + " s2s = " + s2s);
            return "registration_id = " + registration_id + "  s2s = " + s2s;
        }

        [HttpGet("ftd")]
        public ActionResult<string> ftd([FromQuery] string s2s, [FromQuery] string deposit_id, [FromQuery] string amount)
        {
            System.IO.File.AppendAllText(@"C:\POSTBACK\FTD.txt", "\r\n[" + DateTime.Now + "] deposit_id = " + deposit_id + " s2s = " + s2s + " amount = " + amount);
            return "deposit_id = " + deposit_id + " amount = " + amount + "  s2s = " + s2s;
        }
    }
}
