using DatabaseContext;
using DatabaseContextCore;
using DataLayer;
using Google.Apis.YouTube.v3.Data;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
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

        [HttpGet("viwerprofile")]
        public async Task<ActionResult<object>> ViwerProfileAsync([FromQuery] string localUserToken, [FromQuery] string streamerid)
        {
            if(string.IsNullOrWhiteSpace(streamerid) ||
                streamerid == "null" ||
                streamerid == "undefined")
            {
                return BadRequest();
            }
            var viewerId = JwtManager.GetClaim(localUserToken, ClaimNames.Username);
            //var _token = JwtManager.GetClaim(localUserToken, ClaimNames.UserToken);
            var LoginPlatform = JwtManager.GetClaim(localUserToken, ClaimNames.LoginPlatform);

            var db = await UserDatabase.GetGivewayDBAsync(_context, streamerid);
            
            var _token = await db.GetTwitchToken();
            var viewer =  await db.GetViewerAsync(viewerId);
            var viewerProfile = await db.GetViewerWallet(viewerId);

            bool IsMember = false;
            switch (LoginPlatform)
            {
                case "Twitch":
                    var TWsubs = db.GetStreamerTwitchSubscribers();
                    if (TWsubs.Any(x => x.user_id == viewerId))
                    {
                        IsMember = true;
                    }
                    break;
                case "YouTube":
                    var YTsubs = db.GetStreamerYoutubeSubscribers();
                    if (YTsubs.Any(x => x.MemberId == viewerId))
                    {
                        IsMember = true;
                    }
                    break;
            }

            return Ok(new { coins = viewerProfile.Coins, isMember = IsMember, name = viewer.Name, email = viewer.Email, isVerified = viewer.IsActive });
        }

        [HttpGet("viwerloginYoutube")]
        public async Task<ActionResult<object>> ViwerLoginYoutubeAsync([FromQuery] string googleToken, [FromQuery] string googleName = "", [FromQuery] string googleEmail = "")
        {
            var viewerId = await ViewerAPIRequest.GetYoutubeIdAsync(googleToken);        

            var db = await UserDatabase.GetGivewayDBAsync(_context);

            var viewer = await db.GetViewerAsync(viewerId);
            if(viewer == null)
            {
                await db.CreateUser(viewerId, googleName, googleEmail, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            }

            var profilePicture = await ViewerAPIRequest.GetYoutubeProfilePictureAsync(viewerId, googleToken);

            var _token = JwtManager.GenerateViewerLoginToken(viewerId,googleToken,"YouTube");
            return Ok(new { success = true, token = _token, profilePicture = profilePicture });
        }

        [HttpGet("viwerloginTwitch")]
        public async Task<ActionResult<object>> ViwerLoginTwitchAsync([FromQuery] string twitchToken, [FromQuery] string twitchName = "", [FromQuery] string twitchId = "", [FromQuery] string twitchEmail = "")
        {
            
            var db = await UserDatabase.GetGivewayDBAsync(_context);
            var viewer = await db.GetViewerAsync(twitchId);
            if (viewer == null)
            {
                await db.CreateUser(twitchId, twitchName, twitchEmail, Request.HttpContext.Connection.RemoteIpAddress.ToString());               
            }            

            var _token = JwtManager.GenerateViewerLoginToken(twitchId, twitchToken,"Twitch");
            return Ok(new { success=true, token = _token, profilePicture = "" });
        }

        [HttpGet("hystory")]
        public async Task<ActionResult<object>> ViwerHystoryAsync([FromQuery] string localUserToken, [FromQuery] string streamerid, [FromQuery] int pageId)
        {
            int itemPerPage = 10;
            var userid = "";
            try
            {
                userid = JwtManager.GetClaim(localUserToken, ClaimNames.Username);
            }
            catch
            {
                return BadRequest(new { status = false, reason = "Invalid Login" });
            }
            List<dynamic> dynamics = new List<dynamic>();

            var db = await UserDatabase.GetGivewayDBAsync(_context,streamerid);
            var hs = db.GetAllRedeems();
            var hystory = hs.Where(x => x.UserId == userid).OrderByDescending(x=>x.TimeOfWin).Skip(pageId * itemPerPage).Take(itemPerPage).ToList();

            foreach(var item in hystory)
            {
                dynamics.Add(new
                {
                    itemName = item.ShopItem.Nume,
                    timeOfWin = item.TimeOfWin
                });
            }

            return Ok(new { hystory = dynamics });
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

            if (itemid == "quikesustinator")
            {
                var gt = await db.GetGivewayTikets();
                if (!gt.Any(x => x.ViewerID == userid && x.GiveawayID == 1059))
                {
                    return Ok(new { status = false, reason = "Trebuie sa-l sustii pe CasinosRo sa poti deschide acest pachet!" });
                }
            }

            var utilizator = await db.GetViewerAsync(userid);
            bool IsMember = db.GetStreamerYoutubeSubscribers().Any(x => x.MemberId == utilizator.Id);
                        
            if (utilizator.IsActive == false && IsMember == false)
            {
                return Ok(new { status = false, reason = "You must validate your account to be able to buy from the site!" });
            }

            var shop = await db.GetShopAsync();
            var item = shop.FirstOrDefault(x => x.ItemID == itemid);

            if (item == null)
            {
                return Ok(new { status = false, reason = "Product not found!" });
            }

            if (item.OnlyMembers == true)
            {
                if (!IsMember)
                {
                    return Ok(new { status = false, reason = "The product can only be bought by subscribers/members of this channel!" });
                }
            }

            if (item.Stoc < 1)
            {
                return Ok(new { status = false, reason = "Unfortunately, the product is no longer in stock!" });
            }

            if (db.IsUserOnCooldown(userid, "shop"))
            {
                return Ok(new { status = false, reason = "You can use the shop once every 5 seconds!" });
            }

            db.AddUserOnCooldown(userid, "shop", 0.08);

            if (db.IsUserOnCooldown(utilizator.Id, item.ItemID))
            {
                return Ok(new { status = false, reason = "You can buy this product at the date and time -" + db.sGetUserCooldown(utilizator.Id, item.ItemID) });
            }

            if (utilizator.Wallets.FirstOrDefault(x => x.StreamerId == db.GetAccountID()).Coins < item.Pret)
            {
                return Ok(new { status = false, reason = $"You do not have enough {db.GetStreamerSettings().LoyaltySettings.LoyaltyName} for this product!" });
            }
            else
            {               
                await db.RemovePointsToOneUser(utilizator, item.Pret, false);
            }

            db.AddUserOnCooldown(utilizator.Id, item.ItemID, item.Cooldown);

            if (item.ItemType == ItemType.NormalItem)
            {
                db.RedeemItem(utilizator, item, item.Nume);
                await Startup.YoutubeChatWriter.WriteMessageAsync($"@{utilizator.Name} just bought [{item.Nume}] from the !shop.");
                return Ok(new { status = true, reason = $"Congratulations on your purchase {item.Nume}!" });

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
                    //await Startup.YoutubeChatWriter.WriteMessageAsync($"@{utilizator.Name} just found {reward.Name} inside {item.Nume}, visit the shop and try it yourself.");
                    return Ok(new { status = true, reason = $"Congratulations you won {reward.Name} from {item.Nume}" });
                }
                else if (reward.DropType == DropType.LoyaltyPoints)
                {                    
                    //await Startup.YoutubeChatWriter.WriteMessageAsync($"@{utilizator.Name} just won {reward.DropList}{Settings.ProjectSettings.NumePuncteLoialitate} from {item.Nume} on !shop try it too.");
                    return Ok(new { status = true, reason = await db.WinPointsAsync(utilizator, int.Parse(reward.DropList)) });
                }
                else if (reward.DropType == DropType.Code)
                {
                    db.RedeemItem(utilizator, item, reward.Name);
                    //await Startup.YoutubeChatWriter.WriteMessageAsync($"@{utilizator.Name} just won {reward.Name} from {item.Nume}, visit the shop and try it yourself.");
                    return Ok(new { status = true, reason = $"You won {reward.Name} from {item.Nume} - you get the {reward.Name} on your email." });
                }
                else if(reward.DropType == DropType.Unlucky)
                {
                    return Ok(new { status = true, unlucky=true ,reason = $"You opened {item.Nume} - and you found {reward.Name}. Better luck next time." });
                }
            }

            return Ok(new { status = false, reason = "" });
        }
    }
}
