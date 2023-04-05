using DatabaseContext;
using DatabaseContextCore;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Youtube_Contractor;

namespace StreamApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamerPageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StreamerPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<ActionResult<List<ShopItem>>> GetAsync([FromQuery] string streamerid, [FromQuery] string localUserToken)
        {
            var db = UserDatabase.GetByUsername(streamerid);
            var toReturn = await db.GetShopAsync();

            if(!string.IsNullOrWhiteSpace(localUserToken))
            {
                var userid = JwtManager.GetClaim(localUserToken, ClaimNames.Username);
                if(!string.IsNullOrWhiteSpace(userid) && toReturn?.Count > 0)
                {
                    foreach (var item in toReturn)
                    {
                        item.CooldownValue = db.dGetUserCooldown(userid, item.ItemID);
                    }
                }
            }

            return Ok(toReturn);
        }

        /// <summary>
        /// Get user information for current page.
        /// </summary>
        /// <param name="localUserToken"></param>
        /// <returns></returns>
        [HttpGet("viwerprofile")]
        public async Task<ActionResult<object>> ViwerProfileAsync([FromQuery] string localUserToken, [FromQuery] string streamerid)
        {
            var viewerId = JwtManager.GetClaim(localUserToken, ClaimNames.Username);
            var googleToken = JwtManager.GetClaim(localUserToken, ClaimNames.YoutubeToken);
            var db = await UserDatabase.GetGivewayDBAsync(_context, streamerid);

            if (viewerId == "UCMs4eLGliIOry-Eg-ITECqw")
            {
               // var memberLevel = await YoutubeViewerAuth.GetMemberLevel(await db.GetYoutubeToken(),  viewerId);
            }

            var viewer =  await db.GetViewerAsync(viewerId);
            var viewerProfile = await db.GetViewerWallet(viewerId);

            return Ok(new { coins = viewerProfile.Coins, isMember = viewerProfile.IsMember, name = viewer.Name, email = viewer.Email, isVerified = viewer.IsActive });
        }

        [HttpGet("viwerloginYoutube")]
        public async Task<ActionResult<object>> ViwerLoginYoutubeAsync([FromQuery] string googleToken, [FromQuery] string googleName = "", [FromQuery] string googleEmail = "")
        {
            var viewerId = await YoutubeViewerAuth.GetYoutubeIdAsync(googleToken);        

            var db = await UserDatabase.GetGivewayDBAsync(_context);

            var viewer = await db.GetViewerAsync(viewerId);
            if(viewer == null)
            {
                await db.CreateUser(viewerId, googleName, googleEmail);
            }

            var profilePicture = await YoutubeViewerAuth.GetYoutubeProfilePictureAsync(viewerId, googleToken);

            var _token = JwtManager.GenerateViewerLoginToken(viewerId,googleToken);
            return Ok(new { token = _token, profilePicture = profilePicture });
        }

        [HttpGet("buyitem")]
        public async Task<ActionResult<object>> BuyItemAsync([FromQuery] string streamerid, [FromQuery] string localUserToken, [FromQuery] string itemid)
        {
            var userid = "";
            try
            {
              userid = JwtManager.GetClaim(localUserToken, ClaimNames.Username);
            }
            catch
            {
                return BadRequest(new { status = false, reason = "Invalid Login" });
            }

            var db = await UserDatabase.GetGivewayDBAsync(_context, streamerid);

            var utilizator = await db.GetViewerAsync(userid);
            var IsMember = utilizator.Wallets.FirstOrDefault(x => x.StreamerId == db.GetAccountID()).IsMember;
            //if (!utilizator.Wallets.Any(x => x.StreamerId == db.GetAccountID()))
            //{
            //    await db.CreateUserWallet(new ViewerWallet() { Coins = 0, StreamerId = db.GetAccountID(), ViewerId = userid });
            //}

            if (utilizator.IsActive == false && IsMember == false)
            {
                return Ok(new { status = false, reason = "Trebuie sa-ti validezi contul sa poti cumpara de pe site!" });
            }

            var shop = await db.GetShopAsync();
            var item = shop.FirstOrDefault(x => x.ItemID == itemid);

            if (item == null)
            {
                return Ok(new { status = false, reason = "Produsul nu a fost gasit!" });
            }

            if (item.OnlyMembers == true)
            {
                if (!IsMember)
                {
                    return Ok(new { status = false, reason = "Produsul poate fi cumparat doar de membrii!" });
                }
            }

            if (item.Stoc < 1)
            {
                return Ok(new { status = false, reason = "Din pacate produsul nu mai este in stoc!" });
            }

            if (db.IsUserOnCooldown(userid, "shop"))
            {
                return Ok(new { status = false, reason = "Poti folosi shopul o data la 5 de secunde!" });
            }

            db.AddUserOnCooldown(userid, "shop", 0.08);

            if (db.IsUserOnCooldown(utilizator.Id, item.ItemID))
            {
                return Ok(new { status = false, reason = "Mai poti cumpara acest produs la data ora - " + db.sGetUserCooldown(utilizator.Id, item.ItemID) });
            }

            if (utilizator.MemberLevel > MemberLevels.Coxumator)
            {
                item.Pret = (int)(item.Pret * 0.8);
            }

            if (utilizator.Wallets.FirstOrDefault(x => x.StreamerId == db.GetAccountID()).Coins < item.Pret)
            {
                return Ok(new { status = false, reason = $"Nu ai suficiente {ProjectSettings.NumePuncteLoialitate} pentru acest produs" });
            }
            else
            {               
                await db.AddPointsToOneUser(utilizator.Id, -1 * item.Pret, false);
            }

            db.AddUserOnCooldown(utilizator.Id, item.ItemID, item.Cooldown);

            if (item.ItemType == ItemType.NormalItem)
            {
                db.RedeemItem(utilizator, item, item.Nume);
                await Startup.YoutubeChatWriter.WriteMessageAsync($"@{utilizator.Name} tocmai a cumparat [{item.Nume}] de pe !shop.");
                return Ok(new { status = true, reason = $"Felicitari ai cumparat {item.Nume}!" });

            }

            if (item.ItemType == ItemType.MysteryBox)
            {
                int maxLuck = item.Drops.Sum(d => d.Luck);
                int number = new Random().Next(1, maxLuck);
                DropItem reward = new DropItem();

                int a = -1;
                foreach (var x in item.Drops)
                {
                    if (number > a && number <= a + x.Luck)
                    {
                        reward = x;
                        break;
                    }
                    a += x.Luck;
                }

                if (reward.DropType == DropType.Normal)
                {
                    db.RedeemItem(utilizator, item, reward.Name);
                    await Startup.YoutubeChatWriter.WriteMessageAsync($"@{utilizator.Name} tocmai a castigat {reward.Name} din {item.Nume} pe !shop incearca si tu.");
                    return Ok(new { status = true, reason = $"Felicitari ai castigat {reward.Name} din {item.Nume}" });
                }
                else if (reward.DropType == DropType.LoyaltyPoints)
                {                    
                    await Startup.YoutubeChatWriter.WriteMessageAsync($"@{utilizator.Name} tocmai a castigat {reward.DropList}{Settings.ProjectSettings.NumePuncteLoialitate} din {item.Nume} pe !shop incearca si tu.");
                    return Ok(new { status = true, reason = await db.WinPointsAsync(utilizator, int.Parse(reward.DropList)) });
                }
                else if (reward.DropType == DropType.Code)
                {
                    db.RedeemItem(utilizator, item, reward.Name);
                    await Startup.YoutubeChatWriter.WriteMessageAsync($"@{utilizator.Name} tocmai a castigat {reward.Name} din {item.Nume} pe !shop incearca si tu.");
                    return Ok(new { status = true, reason = $"Felicitari ai castigat {reward.Name} din {item.Nume} - primesti pe email premiul." });
                }
            }

            return Ok(new { status = false, reason = "" });
        }
    }
}
