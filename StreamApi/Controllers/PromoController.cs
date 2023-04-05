using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace StreamApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PromoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getallpromo")]
        public async Task<ActionResult<List<Promo>>> GetAllPromoAsync([FromQuery] string streamerid)
        {
            var db = await UserDatabase.GetGivewayDBAsync(_context, streamerid);          
            return db.GetUserPromos();
        }

        [HttpGet("hasclicked")]
        public async Task<ActionResult<bool>> hasclickedAsync([FromQuery] string streamerid, [FromQuery] string promoname, [FromQuery] string? user)
        {
            var db = await UserDatabase.GetGivewayDBAsync(_context, streamerid);
            var up = db.GetUserPromos();
            var upClick = db.GetUserPromosClicks();

            upClick ??= new List<PromoClicks>();
            
            if (!upClick.Any(x=>x.PromoName == promoname))
            {
                var q = up.FirstOrDefault(x => x.name == promoname);
                upClick.Add(new PromoClicks()
                {
                    PromoName = promoname,  
                    Clicks = new List<Click>()
                });
            }
            
            upClick.FirstOrDefault(x => x.PromoName == promoname).Clicks.Add(new Click()
            {
                ClickTime = DateTime.Now,
                IpAdress = Request.HttpContext.Connection.RemoteIpAddress.ToString()
            });

            return db.SaveUserPromosClick(upClick);
        }

        [HttpGet("getonepromo")]
        public async Task<ActionResult<Promo>> GetOnePromoAsync([FromQuery] string streamerid, [FromQuery] string promoname)
        {
            var db = await UserDatabase.GetGivewayDBAsync(_context, streamerid);
            return db.GetUserPromos().FirstOrDefault(x=>x.name == promoname);
        }

        [HttpPost("savepromo")]
        public async Task<ActionResult<bool>> SavePromosAsync([FromQuery] string token, [FromBody] List<Promo> promos)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return db.SaveUserPromos(promos);
            }

            return Ok(false);
        }
    }
}
