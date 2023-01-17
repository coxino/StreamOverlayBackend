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
        public async Task<ActionResult<List<ShopItem>>> GetAsync([FromQuery] string username, [FromQuery] string viewerId = "")
        {
            var db = await UserDatabase.GetGivewayDBAsync(_context);
            if (db.ValidationResponse.ValidationResponse != ValidationResponse.Success)
            {
                return BadRequest("Din pacate nu pot valida requestul tau!");
            }

            var toReturn = await db.GetShopAsync(viewerId);
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

        [HttpPost("cumpara")]
        public async Task<ActionResult<string>> BuyItemAsync([FromBody] ShopRequestModel userModel)
        {
            var db = await UserDatabase.GetGivewayDBAsync(_context);
            if (db.ValidationResponse.ValidationResponse != ValidationResponse.Success)
            {
                return "Din pacate nu pot valida requestul tau!";
            }

            var userid = JwtManager.GetClaim(userModel.localUserToken, ClaimNames.Username);

            var utilizator = await db.GetViewerAsync(userid);           

            if (utilizator.IsActive == false && utilizator.MemberLevel < MemberLevels.Coxumator)
            {
                return "Trebuie sa-ti validezi contul sa poti cumpara de pe site!";
            }

            var shop = await db.GetShopAsync();
            var item = shop.FirstOrDefault(x => x.ItemID == userModel.item.ItemID && x.Nume == userModel.item.Nume);

            if (item == null)
            {
                return "Produsul nu a fost gasit!";
            }

            if (item.OnlyMembers == true)
            {
                if (utilizator.MemberLevel < MemberLevels.Coxumator)
                {
                    return "Produsul poate fi cumparat doar de membrii!";
                }
            }

            if (item.Stoc < 1)
            {
                return "Din pacate produsul nu mai este in stoc!";
            }

            if (db.IsUserOnCooldown(userid, "shop"))
            {
                return Ok("Poti folosi shopul o data la 5 de secunde!");
            }

            db.AddUserOnCooldown(userid, "shop", 0.08);

            if (db.IsUserOnCooldown(utilizator.Id, item.ItemID))
            {
                return "Mai poti cumpara acest produs la data ora - " + db.GetUserCooldown(utilizator.Id, item.ItemID);
            }

            if (utilizator.MemberLevel > MemberLevels.Coxumator)
            {
                item.Pret = (int)(item.Pret * 0.8);
            }

            if (utilizator.UserCox < item.Pret)
            {
                return $"Nu ai suficiente {ProjectSettings.NumePuncteLoialitate} pentru acest produs";
            }
            else
            {
                await db.AddPointsToOneUser(utilizator.Id, -1 * item.Pret, false);
            }

            db.AddUserOnCooldown(utilizator.Id, item.ItemID, item.Cooldown);

            if (item.ItemID == "mystery2")
            {
                int number = new Random().Next(0, 11);
                if (number == 4)
                {
                    db.RedeemItem(utilizator, item, "50 SHINING CROWN");
                    System.IO.File.AppendAllText(@"C:\StreamOverlay\assets\img\test.txt", "\r\n " + utilizator.Name);
                    await Startup.YoutubeChatWriter.WriteMessageAsync($"@{utilizator.Name} tocmai a castigat 50 RON rulaj 1x din [{item.Nume}] pe !shop incearca si tu.");
                    return Ok("Ai primit 50RON SHINING CROWN! Nu uita sa-ti completezi numele de la superbet!");
                }
                else
                {
                    return "NECASTIGATOR";
                }
            }

            if (item.ItemID == "mystery1")
            {
                int number = new Random().Next(0, 20);
                if (number == 14)
                {
                    db.RedeemItem(utilizator, item, "50 SHINING");
                    await Startup.YoutubeChatWriter.WriteMessageAsync($"@{utilizator.Name} tocmai a castigat 50 RON rulaj 1x din [{item.Nume}] pe !shop incearca si tu.");
                    return Ok("Ai primit 50RON SHINING CROWN! Nu uita sa-ti completezi numele de la superbet!");
                }
                else
                {
                    return "NECASTIGATOR";
                }
            }

            if (item.ItemID == "gift100")
            {         
                int number = new Random().Next(0, 300);               

                if (number < 150)
                {
                    return await db.WinPointsAsync(utilizator, 50);
                }

                if (number < 250)
                {
                    return await db.WinPointsAsync(utilizator, 100);
                }

                if (number < 300)
                {
                    return await db.WinPointsAsync(utilizator, 250);
                }                
            }


            await Startup.YoutubeChatWriter.WriteMessageAsync($"@{utilizator.Name} tocmai a cumparat [{item.Nume}] de pe !shop, strange {ProjectSettings.NumePuncteLoialitate} si cumpara si tu.");
            return db.RedeemItem(utilizator, item, userModel.item.OptionalData);
        }

        [HttpGet("validare")]
        public async Task<ActionResult<string>> ValidateAccountAsync([FromQuery] string valcode)
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
                        return RedirectPermanent("https://coxino.ro/shop?alert=CONTUL-A-FOST-VALIDAT");
                }
                else
                {
                    return RedirectPermanent("https://coxino.ro/shop?alert=CONTUL-A-FOST-VALIDAT");
                }
            }

            return RedirectPermanent("https://coxino.ro/shop?alert=VALIDAREA-A-AVUT-ERORI");
        }


        [HttpGet("genvalcode")]
        public async Task<ActionResult<string>> GenValidCodeAsync([FromQuery] string userID)
        {
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

            if(user == null)
            {
                return Ok(new { msj = "Nu pot gasi contul in baza de date!" }); ;
            }

            if(user.IsActive == true)
            {
                return Ok(new { msj = "Contul a fost deja validat!." });
            }

            if (user != null)
            {
                var code = JwtManager.GenerateViewerToken(userID);
                Email(user.Email, generateEmail(code, user.Name), "Validare cont coxino.ro");
                if (string.IsNullOrWhiteSpace(user.EmailSecundar) == false)
                {
                    Email(user.EmailSecundar, generateEmail(code, user.Name), "Validare cont coxino.ro");
                    return Ok(new { msg = $"Am trimis e-mail-ul pe adresele {user.EmailSecundar} si {user.Email}, poate dura pana la 5 minute. Verifica si spam!" });
                }
            }

            return Ok(new { msg = $"Am trimis e-mail-ul pe adresa {user.Email}, poate dura pana la 5 minute. Verifica si spam!" });
        }

        private string generateEmail(string code, string name)
        {
            var link = "https://coxino.go.ro:5000/api/shop/validare?valcode=";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<h1>Salut {name} </h1>");
            var q = System.IO.File.ReadAllText(@"C:\oferta\oferta1.txt");
            sb.AppendLine($"<h2>Pentru a activa contul este necesar sa dai click <b> <a href='{link}{code}'>AICI</a> </b><br></h2>");
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
