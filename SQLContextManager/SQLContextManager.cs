using DatabaseContext;
using JWTManager;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SQLContextManager
{
    public class SQLContex
    {
        private ApplicationDbContext _context;
        private Guid UserId;



        public SQLContex()
        {
        }

        public SQLContex(ApplicationDbContext context, Guid userId)
        {
            _context = context;
            UserId = userId;
        }

        public async Task<List<Viewer>> GetLoyalityAsync()
        {
            //.Where(x=>x.User.Id == UserId)
            var q = await _context.Viewers.ToListAsync();
            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ammount"></param>
        /// <returns>success</returns>
        public async Task<bool> AddPointToAllViewersAsync(int ammount)
        {
            var usersLoyalty = await GetLoyalityAsync();
            var userLoyaltyAux = usersLoyalty;
            foreach (var user in userLoyaltyAux)
            {
                var existingChild = user;
                if (existingChild.LastActive.AddMinutes(15) > DateTime.Now)
                {
                    existingChild.Inventory += ammount;
                    _context.Entry(user).CurrentValues.SetValues(existingChild);
                }
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> AddPointToAllViewersAndCountAsync(int ammount)
        {
            int count = 0;
            var usersLoyalty = await GetLoyalityAsync();
            var userLoyaltyAux = usersLoyalty;
            foreach (var user in userLoyaltyAux)
            {
                var existingChild = user;
                if (existingChild.LastActive.AddMinutes(15) > DateTime.Now)
                {
                    count++;
                    existingChild.Inventory += ammount;
                    _context.Entry(user).CurrentValues.SetValues(existingChild);
                }
            }

            return await _context.SaveChangesAsync() > 0 ? count : -1;
        }

        public async Task<List<GivewayModel>> GetGiveawayList()
        {
            return await _context.Giveways.ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewer"></param>
        /// <returns>bool success</returns>
        public async Task<bool> SaveUser(Viewer viewer)
        {
            if (await _context.Viewers.AnyAsync(x => x.Id == viewer.Id))
            {
                var entity = await _context.Viewers.FirstOrDefaultAsync(x => x.Id == viewer.Id);
                _context.Entry(entity).CurrentValues.SetValues(viewer);
                return await _context.SaveChangesAsync() > 0;
            }
            else
            {
               return await CreateUser(viewer);
            }            
        }

        public async Task<bool> CreateGivewayTiket(Viewer viewer, GivewayModel giveway)
        {
            if (await _context.Viewers.AnyAsync(x => x.Id == viewer.Id))
            {
                if (await _context.Giveways.AnyAsync(x => x.Id == giveway.Id))
                {
                    await _context.GivewayTikets.AddAsync(new GivewayTiket()
                    {
                        GiveawayID = giveway.Id,
                        ViewerID = viewer.Id
                    });

                    return await _context.SaveChangesAsync() > 0;

                }
            }

            return false;
        }

        public async Task SetGiveawayWinner(int id)
        {
            var giveaway = await _context.Giveways.FirstOrDefaultAsync(x => x.Id == id);
            var all_tikets = await _context.GivewayTikets.ToListAsync();
            var thisGWTikets = all_tikets.Where(x => x.GiveawayID == id).ToList();

            var all_tikets_distinct = all_tikets.GroupBy(x => x.ViewerID)
                                  .Select(g => g.First())
                                  .ToList();

            List<string> vs = new List<string>();

            while (vs.Count() < giveaway.WinnersCount)
            {
                var winnerNumber = 0;
                var winner = "";

                winnerNumber = new Random().Next(0, thisGWTikets.Count());
                winner = thisGWTikets[winnerNumber].ViewerID;

                if (vs.Any(x => x == winner) == false)
                {
                    vs.Add(winner);
                    await _context.Winners.AddAsync(new Winner() { GiveawayID = id, ViewerID = winner });
                }

                if (all_tikets_distinct.Count() <= giveaway.WinnersCount)
                {
                    break;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<string> SetUserActive(string viewerID)
        {
            if(await ViewerExist(viewerID))
            {
                var viewer = await GetUserLoyalty(viewerID);
                viewer.LastActive = DateTime.Now;
                return await SaveUser(viewer) ? "success" : "failed";
            }

            return "User Inexistent";
        }

        public async Task<List<string>> GetGiveawayWinners(int id)
        {
            List<string> _winners = new List<string>();
            var winners = await _context.Winners.Where(x => x.GiveawayID == id).Select(x => x.ViewerID).ToListAsync();
            foreach(var winner in winners)
            {
                var viewer = await _context.Viewers.FirstOrDefaultAsync(x => x.Id == winner);
                _winners.Add(viewer.Name);
            }
            return _winners;
        }

        public async Task<int> GetGivewayTiketCount(int givewayID)
        {
            return await _context.GivewayTikets.CountAsync(x => x.GiveawayID == givewayID);
        }

        public async Task<int> GetViewerGivewayTokens(string userID, int givewayID)
        {
            return await _context.GivewayTikets.CountAsync(x => x.GiveawayID == givewayID && x.ViewerID == userID);
        }

        public async Task<GivewayModel> GetGivewayModel(int givewayID)
        {
           return await _context.Giveways.FirstOrDefaultAsync(x => x.Id == givewayID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<Viewer> GetUserLoyalty(string userID)
        {
            return await _context.Viewers.FirstOrDefaultAsync(x => x.Id == userID);
        }

        public async Task<bool> AddPointToViewerAsync(Viewer viewer, int ammount)
        {
            viewer.Inventory += ammount;
            return await SaveUser(viewer);
        }

        public async Task<bool> ViewerExist(string userID)
        {
            return await _context.Viewers.AnyAsync(x => x.Id == userID);
        }

        public async Task<bool> CreateUser(Viewer viewer)
        {
            await  _context.Viewers.AddAsync(viewer);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> ReadUserJackpot()
        {
            //_context.Jackpots.
            return 10000;
        }

        public async Task<bool> AddPointToViewerAsync(string userID, int ammount)
        {
            var viewer = await GetUserLoyalty(userID);
            return await AddPointToViewerAsync(viewer, ammount);
        }
    }
}
