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
                    existingChild.UserCox += ammount;
                    _context.Entry(user).CurrentValues.SetValues(existingChild);
                }
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public string GetAccountID()
        {
            return UserId.ToString();
        }

        public async Task<int> AddPointToAllViewersAndCountAsync(int ammount)
        {
            int count = 0;
            var usersLoyalty = await GetLoyalityAsync();
            var userLoyaltyAux = usersLoyalty;
            foreach (var user in userLoyaltyAux)
            {
                var existingChild = user;
                if (existingChild.LastActive.AddMinutes(20) > DateTime.Now)
                {
                    count++;
                    switch (existingChild.MemberLevel)
                    {
                        case MemberLevels.Ajutor:
                        case MemberLevels.Cop:
                        case MemberLevels.Coxumator:
                        case MemberLevels.Gangster:
                        case MemberLevels.ElChapo:
                            existingChild.UserCox += ammount * 2;
                            break;
                        default:
                            existingChild.UserCox += ammount;
                            break;
                    }                   
                    _context.Entry(user).CurrentValues.SetValues(existingChild);
                }
            }

            return await _context.SaveChangesAsync() > 0 ? count : -1;
        }

        public async Task RemoveBroadcastMessageAsync(Viewer viewer)
        {
            var v2 = viewer;
            v2.BroadcastMessageCount -= 1;
            _context.Entry(viewer).CurrentValues.SetValues(v2);
            _ = await _context.SaveChangesAsync();
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

            List<GivewayTiket> membersTikets = new List<GivewayTiket>();
            foreach(var viewerTiket in thisGWTikets)
            {
                var viewer = await GetViewerModel(viewerTiket.ViewerID);
                if (viewer.MemberLevel > MemberLevels.Moderator)
                {
                    membersTikets.Add(new GivewayTiket() { ViewerID = viewer.Id, GiveawayID = id });
                }
            }

            thisGWTikets.AddRange(membersTikets);

            var all_tikets_distinct = all_tikets.GroupBy(x => x.ViewerID)
                                  .Select(g => g.First())
                                  .ToList();

            List<string> giveawayWinners = new List<string>();

            await Task.Run(async () =>
            {
                while (giveawayWinners.Count() < giveaway.WinnersCount)
                {
                    var winnerNumber = 0;
                    var winner = "";

                    winnerNumber = new Random().Next(0, thisGWTikets.Count());
                    winner = thisGWTikets[winnerNumber].ViewerID;

                    if (giveawayWinners.Any(x => x == winner) == false)
                    {
                        giveawayWinners.Add(winner);
                        await _context.Winners.AddAsync(new Winner() { GiveawayID = id, ViewerID = winner });
                    }

                    if (all_tikets_distinct.Count() <= giveaway.WinnersCount)
                    {
                        break;
                    }
                }
            });
            await _context.SaveChangesAsync();
        }

        public async Task<string> ValidateUser(string accountId)
        {
            if (await ViewerExist(accountId))
            {
                var viewer = await GetViewerModel(accountId);
                viewer.IsActive = true;
                return await SaveUser(viewer) ? "success" : "failed";
            }

            return "Failed to Validate user";
        }

        public async Task<string> SetUserActive(string viewerID, string viewerName)
        {
            if(await ViewerExist(viewerID))
            {
                var viewer = await GetViewerModel(viewerID);
                if(viewer.Name != viewerName)
                {
                    viewer.Name = viewerName;
                }
                viewer.LastActive = DateTime.Now;
                return await SaveUser(viewer) ? "success" : "failed";
            }

            else
            {
                Viewer newViewer = new Viewer()
                {
                    LastActive = DateTime.Now,
                    CreationTime = DateTime.Now,
                    Email = "",
                    Id = viewerID,
                    UserCox = 25,
                    Ipadress = "",
                    MemberLevel = MemberLevels.Viewer,
                    Name = viewerName,
                    SuperbetName = "",
                    IsActive = false,
                    BroadcastMessageCount = 0,
                    ExpiresMember = DateTime.Now
                };

                return await CreateUser(newViewer) ? $"{viewerName} bine ai venit pe live, acum esti inscris(a) in sistemul de loialitate, intra pe https://coxino.ro/shop pentru a castiga multe premii" +
                    $" faine" : $"{viewerName} nu am putut sa te inscriu in sistemul de loialitate. Incearca direct pe site https://coxino.ro/shop";
            }
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
        public async Task<Viewer> GetViewerModel(string userID)
        {
            return await _context.Viewers.FirstOrDefaultAsync(x => x.Id == userID);
        }

        public async Task<bool> AddPointToViewerAsync(Viewer viewer, int ammount, bool doubleUp = true)
        {
            if (doubleUp == true)
            {
                switch (viewer.MemberLevel)
                {
                    case MemberLevels.Ajutor:
                    case MemberLevels.Cop:
                    case MemberLevels.Coxumator:
                    case MemberLevels.Gangster:
                    case MemberLevels.ElChapo:
                        ammount *= 2;
                        viewer.UserCox += ammount;
                        break;
                    default:
                        viewer.UserCox += ammount;
                        break;
                }
            }
            else
            {
                viewer.UserCox += ammount;
            }
               
            return await SaveUser(viewer);
        }

        public async Task<bool> ViewerExist(string userID)
        {
            return await _context.Viewers.AnyAsync(x => x.Id == userID);
        }

        public async Task<List<Viewer>> GetViewerByNameAsync(string userName)
        {
            return await _context.Viewers.ToListAsync();
        }

        public async Task<bool> CreateUser(Viewer viewer)
        {
            await  _context.Viewers.AddAsync(viewer);

            try
            {

                return await _context.SaveChangesAsync() > 0;
            }catch(Exception ex)
            {
                var x = ex.Message;
                return false;
            }
        }

        public async Task<int> ReadUserJackpot()
        {
            //_context.Jackpots.
            return 0;
        }

        public async Task<bool> AddPointToViewerAsync(string userID, int ammount, bool doubleUp = true)
        {
            var viewer = await GetViewerModel(userID);
            return await AddPointToViewerAsync(viewer, ammount,doubleUp);
        }

        public async Task<bool> SetUserLevel(string id, MemberLevels level, DateTime expires)
        {
            var viewer = await GetViewerModel(id);
            viewer.MemberLevel = level;
            viewer.ExpiresMember = expires;
            return await SaveUser(viewer);
        }
    }
}
