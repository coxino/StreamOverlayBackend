using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Mvc;
using SQLContextManager;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult<List<ShopItem>> Get([FromQuery] string username)
        {
            var toReturn = UserDatabase.GetByUsername(username).GetShop();
            return Ok(toReturn);
        }

        [HttpGet("stoc")]
        public int Stoc(string username)
        {
            return UserDatabase.GetByUsername(username).RedeemOfItem("Bilet PROMO 25 PSF");
        }


        [HttpPost("cumpara")]
        public async Task<ActionResult<string>> BuyItemAsync([FromBody] UserUpdateModel userModel)
        {
            var db = await UserDatabase.GetDatabaseAsync(userModel.token, _context);
            if (db.ValidationResponse.ValidationResponse != ValidationResponse.Success)
            {
                return BadRequest("Din pacate nu pot valida requestul tau!");
            }

            var item = db.GetShop().FirstOrDefault(x => x.ItemID == userModel.itemID);
            if (item.Stoc < 1)
            {
                return "Din pacate produsul nu mai este in stoc!";
            }
            if (item == null)
            {
                return "Produsul nu a fost gasit!";
            }

            if (db.IsUserOnCooldown(userModel.userID, "shop"))
            {
                return Ok("Poti folosi shopul o data la 5 de secunde!");
            }

            db.AddUserOnCooldown(userModel.userID, "shop", 0.08);

            var utilizator = await db.GetViewerAsync(userModel.userID);
            if (item.ItemID == "gift100")
            {
                if (db.IsUserOnCooldown(userModel.userID, "gift100") == false)
                {
                    db.AddUserOnCooldown(userModel.userID, "gift100", int.MaxValue);
                    int number = new Random().Next(0, 400);
                    if (number == 14)
                    {
                        await db.RedeemItem(utilizator, item);
                        return Ok("Ai primit Paysafe de 25lei puncte!");
                    }

                    if (number == 11)
                    {
                        return await db.WinPointsAsync(utilizator, 1000);
                    }

                    if (number >= 350)
                    {
                        return await db.WinPointsAsync(utilizator, 100);
                    }

                    if (number >= 300)
                    {
                        return await db.WinPointsAsync(utilizator, 50);
                    }

                    if (number > 14)
                    {
                        return await db.WinPointsAsync(utilizator, 25);
                    }
                    if (number < 14)
                    {
                        return await db.WinPointsAsync(utilizator, 250);
                    }
                }
                else
                {
                    return Ok("Ai primit deja acest cadou, incearca si maine!");
                }
            }

            if(item.ItemID == "rulaj50")
            {
                if(db.IsUserOnCooldown(userModel.userID,"rulaj50"))
                {
                    return Ok("Te poti inscrie doar o singura data!");
                }
                else
                {
                    db.AddUserOnCooldown(userModel.userID, "rulaj50",int.MaxValue);
                }
            }

            return await db.RedeemItem(utilizator, item, userModel.numeSpeciala);
        }

        //[HttpGet("cumpara")]
        //public async Task<ActionResult<string>> BuyItemAsync([FromQuery] string viewerID, [FromQuery] string itemID, [FromQuery] string userIP)
        //{
        //    var item = UserDatabase.GetDatabase("coxino").GetShop().FirstOrDefault(x => x.ItemID == itemID);
        //    if (item != null)
        //    {
        //        if (UserDatabase.GetDatabase("coxino").IsUserOnCooldown(viewerID, "shop"))
        //        {
        //            return "Poti folosi shopul o data la 5 !";
        //        }
        //        UserDatabase.GetDatabase("coxino").AddUserOnCooldown(viewerID, "shop", 0.5);

        //        //var user = UserDatabase.GetDatabase("coxino").GetLoyaltyPoints().Users.FirstOrDefault(x => x.id == userID);
        //        var user = await _context.GetViewerAsync(new Guid(), viewerID);

        //        if (user != null)
        //        {
        //            if (user.Inventory >= item.Pret)
        //            {
        //                user.Inventory -= item.Pret;
        //                if (item.ItemID == "gift100")
        //                {
        //                    //bah bah
        //                    if (UserDatabase.GetDatabase("coxino").IsUserOnCooldown(viewerID, "gift100") == false)
        //                    {
        //                        UserDatabase.GetDatabase("coxino").AddUserOnCooldown(viewerID, "gift100", int.MaxValue);
        //                        int number = new Random().Next(0, 400);
        //                        if (number == 14)
        //                        {
        //                            UserDatabase.GetDatabase("coxino").RedeemItem(user, item);
        //                            return Ok("Ai primit Paysafe de 25lei puncte!");
        //                        }

        //                        if (number == 11)
        //                        {
        //                            user.Inventory += 1000;
        //                            //UserDatabase.GetDatabase("coxino").SaveUserLoyalty(user);

        //                            return Ok("Ai primit 1000 puncte!");
        //                        }

        //                        if (number >= 350)
        //                        {
        //                            user.Inventory += 100;
        //                            //UserDatabase.GetDatabase("coxino").SaveUserLoyalty(user);

        //                            return Ok("Ai primit 100 puncte!");
        //                        }

        //                        if (number >= 300)
        //                        {
        //                            user.Inventory += 50;
        //                            //UserDatabase.GetDatabase("coxino").SaveUserLoyalty(user);

        //                            return Ok("Ai primit 50 puncte!");
        //                        }

        //                        if (number > 14)
        //                        {
        //                            user.Inventory += 25;
        //                            //UserDatabase.GetDatabase("coxino").SaveUserLoyalty(user);

        //                            return Ok("Ai primit 25 puncte!");
        //                        }
        //                        if (number < 14)
        //                        {
        //                            user.Inventory += 250;
        //                            //UserDatabase.GetDatabase("coxino").SaveUserLoyalty(user);
        //                            return Ok("Ai primit 250 puncte!");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return Ok("Ai primit deja acest cadou, incearca si maine!");
        //                    }
        //                }

        //                if(itemID == "fiftyfifty")
        //                {
        //                    int number = new Random().Next(0, 400);
        //                    if(number > 200)
        //                    {
        //                        user.Inventory += item.Pret;
        //                        if (await _context.SaveChangesAsync() > 0)
        //                        {
        //                            return Ok("Ai castigat!" + item.Pret);
        //                        }
        //                    }
        //                }

        //                if (item.ItemID == "gift1000")
        //                {
        //                    int number = new Random().Next(0, 400);
        //                    if (number == 14)
        //                    {
        //                        UserDatabase.GetDatabase("coxino").RedeemItem(user, new ShopItem() {ItemID = item.ItemID, Nume = "Paysafe25Lei!" });
        //                        return Ok("Ai primit Paysafe de 25lei!");
        //                    }
        //                    if (number == 11)
        //                    {
        //                        int number2 = new Random().Next(0, 200);
        //                        if(number2 == 11)
        //                        {
        //                            UserDatabase.GetDatabase("coxino").RedeemItem(user, new ShopItem() {ItemID = item.ItemID, Nume = "Paysafe50Lei!" });
        //                            return Ok("Ai primit Paysafe de 50lei!");
        //                        }

        //                        if (number2 < 28)
        //                        {
        //                            UserDatabase.GetDatabase("coxino").RedeemItem(user, new ShopItem() { Nume = "Paysafe25Lei!" });
        //                            return Ok("Ai primit Paysafe de 25lei!");
        //                        }
        //                    }
        //                    if (number < 20)
        //                    {
        //                        user.Inventory += 1500;
        //                        if (await _context.SaveChangesAsync() > 0)
        //                        {
        //                            return Ok("Ai primit 1000 puncte!");
        //                        }

        //                    }
        //                    if (number > 390)
        //                    {
        //                        user.Inventory += 2500;
        //                        if (await _context.SaveChangesAsync() > 0)
        //                        {
        //                            return Ok("Ai primit 2000 puncte!");
        //                        }
        //                    }

        //                    if(number > 350)
        //                    {
        //                        user.Inventory += 1000;
        //                        if (await _context.SaveChangesAsync() > 0)
        //                        {
        //                            return Ok("Ai primit 500 puncte!");
        //                        }

        //                    }
        //                    return "Necastigator";
        //                }


        //                if (item.Stoc > 0)
        //                {
        //                    item.Stoc -= 1;
        //                    //UserDatabase.GetDatabase("coxino").SaveUserLoyalty(user);
        //                    UserDatabase.GetDatabase("coxino").RedeemItem(user, item);
        //                    return Ok("Felicitari ai comandat " + item.Nume);
        //                }
        //                else
        //                {
        //                    return Ok("Produsul nu mai este in stoc!");
        //                }
        //            }
        //            else
        //            {
        //                return Ok("Nu ai suficiente puncte!");
        //            }
        //        }
        //    }
        //    return Ok("Produsul nu a fost gasit!");
        //}
    }
}
