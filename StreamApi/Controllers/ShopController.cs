using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Mvc;
using Settings;
using SQLContextManager;
using StaticDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<ActionResult<List<ShopItem>>> GetAsync([FromQuery] string username)
        {
            var db = UserDatabase.GetByUsername(username);

            var toReturn = await db.GetShopAsync();
            return Ok(toReturn);
        }

        [HttpPost("saveshop")]
        public async Task<ActionResult<string>> SaveShopAsync([FromHeader] string token, [FromBody] List<ShopItem> shopItems)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse != ValidationResponse.Success)
            {
                return "Din pacate nu pot valida requestul tau!";
            }


            return Ok(db.SaveShop(shopItems) ? "ShopSalvat" : "error");
        }

        [HttpGet("validare")]
        public async Task<ActionResult<string>> ValidateAccountAsync([FromQuery] string valcode, [FromQuery] string redirect)
        {           
            string accountId = JwtManager.ValidateAccountAsync(valcode, out bool validated);
            if (validated == true && string.IsNullOrWhiteSpace(accountId) == false)
            {
                var db = await UserDatabase.GetGivewayDBAsync(_context);
                var user = await db.GetViewerAsync(accountId);
                if (user.IsActive == false)
                {
                    await db.ValidateUser(user.Id);
                    user = await db.GetViewerAsync(accountId);

                    if (user.IsActive == true)
                        return RedirectPermanent(redirect);
                }
                else
                {
                    return RedirectPermanent(redirect);
                }
            }

            return RedirectPermanent(redirect);
        }


        [HttpGet("genvalcode")]
        public async Task<ActionResult<string>> GenValidCodeAsync([FromQuery] string userID, [FromQuery] string redirect)
        {           
            userID = JwtManager.GetClaim(userID, ClaimNames.Username);

            if (string.IsNullOrWhiteSpace(userID))
            {
                return "error";
            }

            var db = await UserDatabase.GetGivewayDBAsync(_context);
            if (db.IsUserOnCooldown(userID, "validare"))
            {
                return Ok(new { msg = "Trebuie sa astepti 5 minute ca sa mai poti genera un email de validare, verifica si spam-ul intre timp." });
            }
            else
            {
                db.AddUserOnCooldown(userID, "validare", 1);
            }

            var user = await db.GetViewerAsync(userID);

            if (user == null)
            {
                return Ok(new { msg = "Nu pot gasi contul in baza de date!" }); ;
            }

            if (user.IsActive == true)
            {
                return Ok(new { msg = "Contul a fost deja validat!." });
            }

            if (user != null)
            {
                var code = JwtManager.GenerateViewerToken(userID,"");
                Email(user.Email, generateEmail(code, user.Name,redirect), "Validare cont coxino.ro");
                if (string.IsNullOrWhiteSpace(user.EmailSecundar) == false)
                {
                    Email(user.EmailSecundar, generateEmail(code, user.Name,redirect), "Validare cont coxino.ro");
                    return Ok(new { msg = $"Am trimis e-mail-ul pe adresele {user.EmailSecundar} si {user.Email}, poate dura pana la 5 minute. Verifica si spam!" });
                }
            }

            return Ok(new { msg = $"Am trimis e-mail-ul pe adresa {user.Email}, poate dura pana la 5 minute. Verifica si spam!" });
        }

        private string generateEmail(string code, string name,string redirect)
        {
            var link = "https://coxino.go.ro:5000/api/shop/validare?valcode=";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<h1>Salut {name} </h1>");
            var q = System.IO.File.ReadAllText(@"C:\oferta\oferta1.txt");
            sb.AppendLine($"<h2>Pentru a activa contul este necesar sa dai click <b> <a href='{link}{code}&redirect={redirect}'>AICI</a> </b><br></h2>");
            sb.AppendLine(q);
            return sb.ToString();
        }

        public static void Email(string toEmail, string htmlString, string subject)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("noreply@coxino.ro");
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = htmlString;
                smtp.Port = 26;
                smtp.Host = "mail.coxino.ro"; //for gmail host  
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("noreply@coxino.ro", "1qazxsw23edc$RFV");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception e1)
            {
                var p = e1;
            }
        }
    }
}
