using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Coverlay.Controllers
{
    public class GivewayViewModel
    {
        public GivewayModel GivewayModel { get; set; }
        public List<string> Winners { get; set; }
        public int UserTikets { get; set; }
        public int TotalTikets { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class GiveawayController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GiveawayController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<List<GivewayViewModel>> GetAsync([FromQuery] string localUserToken, [FromQuery] string streamerid)
        {
            string viewerID;
            var db = await UserDatabase.GetGivewayDBAsync(_context, streamerid);
            try
            {
                viewerID = JwtManager.GetClaim(localUserToken, ClaimNames.Username);


                List<GivewayViewModel> givewayViewModels = new List<GivewayViewModel>();
                var gs = await db.GetGivewayListAsync();

                foreach (var gaw in gs)
                {
                    var viewer = await db.GetViewerAsync(viewerID);

                    if (viewer == null)
                    {
                        return givewayViewModels.OrderByDescending(x => x.GivewayModel.EndTime > DateTime.Now).ToList();
                    }

                    if (viewer.MemberLevel > MemberLevels.Level2)
                    {
                        gaw.Price = (int)(gaw.Price * 0.8);
                        givewayViewModels.Add(new GivewayViewModel()
                        {
                            GivewayModel = gaw,
                            TotalTikets = await db.SQLContextManager.GetGivewayTiketCount(gaw.Id),
                            UserTikets = await db.SQLContextManager.GetViewerGivewayTokens(viewerID, gaw.Id),
                            Winners = await db.SQLContextManager.GetGiveawayWinners(gaw.Id)
                        });
                    }
                    else
                    {
                        givewayViewModels.Add(new GivewayViewModel()
                        {
                            GivewayModel = gaw,
                            TotalTikets = await db.SQLContextManager.GetGivewayTiketCount(gaw.Id),
                            UserTikets = await db.SQLContextManager.GetViewerGivewayTokens(viewerID, gaw.Id),
                            Winners = await db.SQLContextManager.GetGiveawayWinners(gaw.Id)
                        });
                    }
                }

                var toReturn = givewayViewModels.OrderByDescending(x => x.GivewayModel.EndTime > DateTime.Now).ToList();
                return toReturn;

            }
            catch
            {
                List<GivewayViewModel> givewayViewModels = new List<GivewayViewModel>();
                var gs = await db.GetGivewayListAsync();

                foreach (var gaw in gs)
                {
                    givewayViewModels.Add(new GivewayViewModel()
                    {
                        GivewayModel = gaw,
                        TotalTikets = await db.SQLContextManager.GetGivewayTiketCount(gaw.Id),
                        Winners = await db.SQLContextManager.GetGiveawayWinners(gaw.Id)
                    });
                }

                var toReturn = givewayViewModels.OrderBy(x => x.GivewayModel.EndTime > DateTime.Now).ToList();
                return toReturn;

            }

            return null;
        }

        [HttpGet("setWinner")]
        public async Task<ActionResult<string>> SetWinner([FromQuery] string streamerId, [FromQuery] int giveawayId)
        {
            var db = await UserDatabase.GetGivewayDBAsync(_context, streamerId);
            
                //var gs = await db.GetGivewayListAsync();
                await db.SQLContextManager.SetGiveawayWinner(giveawayId);
                //foreach (var curGiveaway in gs)
                //{
                //    var gcount  = await db.SQLContextManager.GetGiveawayWinners(curGiveaway.Id);
                //    if (gcount.Count() == 0)
                //    {
                //        if (curGiveaway.EndTime < DateTime.Now)
                //        {

                //        }
                //    }
                //}

            return Ok(new {msg = "Cred Ca a mers"});
        }


        [HttpGet("buyTiket")]
        public async Task<ActionResult<string>> BuyTiketAsync([FromQuery] string localUserToken, [FromQuery] int givewayId, [FromQuery] string streamerId)
        {
            string viewerID;
          
            var db = await UserDatabase.GetGivewayDBAsync(_context, streamerId);
            try
            {
                viewerID = JwtManager.GetClaim(localUserToken, ClaimNames.Username);

                List<GivewayViewModel> givewayViewModels = new List<GivewayViewModel>();
                var gs = await db.GetGivewayListAsync();
                var gt = await db.GetGivewayTikets();

                if(gt?.Any(x=>x.ViewerID == viewerID) == true)
                {
                    if(gt.FirstOrDefault(x=>x.ViewerID == viewerID).GiveawayID != givewayId)
                    {
                        return BadRequest(new
                        {
                            msg = "Poti sustine doar un singur streamer!"
                        });
                    }
                }

                var utilizator = await db.GetViewerAsync(viewerID);

                if (utilizator.IsActive == false && !db.GetStreamerYoutubeSubscribers().Any(x=>x.MemberId == utilizator.Id))
                {
                    return BadRequest(new { msg = "Trebuie sa-ti validezi contul sa poti cumpara de pe site!" });
                }

                if (db.IsUserOnCooldown(viewerID, "buyTiket"))
                {
                    return BadRequest(new { msg = "Poti cumpara un ticket la 5 secunde!" });
                }
                db.AddUserOnCooldown(viewerID, "buyTiket", 0.09);
                return Ok(new { msg = await db.BuyGiveawayTiket(utilizator, givewayId) });
            }
            catch
            {
                return BadRequest(new { msg = "Failed to buy giveaway tiket!" });
            }
        }
    }
}
