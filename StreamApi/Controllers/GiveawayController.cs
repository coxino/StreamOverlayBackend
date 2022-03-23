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

        [HttpGet()]
        public async Task<List<GivewayViewModel>> GetAsync([FromQuery] string viewerID = "")
        {
            if (string.IsNullOrWhiteSpace(viewerID))
            {
                var db = await UserDatabase.GetGivewayDBAsync(_context);
                if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
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
            }
            else
            {
                var db = await UserDatabase.GetGivewayDBAsync(_context);
                if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
                {
                    List<GivewayViewModel> givewayViewModels = new List<GivewayViewModel>();
                    var gs = await db.GetGivewayListAsync();

                    foreach (var gaw in gs)
                    {

                        givewayViewModels.Add(new GivewayViewModel()
                        {
                            GivewayModel = gaw,
                            TotalTikets = await db.SQLContextManager.GetGivewayTiketCount(gaw.Id),
                            UserTikets = await db.SQLContextManager.GetViewerGivewayTokens(viewerID, gaw.Id),
                            Winners = await db.SQLContextManager.GetGiveawayWinners(gaw.Id)
                        });
                    }

                    var toReturn = givewayViewModels.OrderByDescending(x => x.GivewayModel.EndTime > DateTime.Now).ToList();
                    return toReturn;
                }
            }
            return null;
        }

        [HttpGet("setWinner")]
        public async Task<ActionResult<string>> SetWinner()
        {
            var db = await UserDatabase.GetGivewayDBAsync(_context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                var gs = await db.GetGivewayListAsync();

                foreach (var gaw in gs)
                {
                    var gcount  = await db.SQLContextManager.GetGiveawayWinners(gaw.Id);
                    if (gcount.Count() == 0)
                    {
                        if (gaw.EndTime < DateTime.Now)
                        {
                            await db.SQLContextManager.SetGiveawayWinner(gaw.Id);
                        }
                    }
                }
            }

            return Ok(new {msg = "Cred Ca a mers"});
        }


        [HttpPost("buyTiket")]
        public async Task<ActionResult<string>> BuyTiketAsync([FromBody] BuyTiketModel buyTiketModel)
        {
            var db = await UserDatabase.GetGivewayDBAsync(_context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                return await db.BuyGiveawayTiket(buyTiketModel);
            }

            return "Nu am putut valida tiketul incearca din nou!";
        }
    }
}
