using DatabaseContext;
using DatabaseContextCore;
using DataLayer;
using JWTManager;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SQLContextManager
{
    public class SQLContex
    {
        private ApplicationDbContext _context;
        private Guid StreamerId;

        public SQLContex()
        {
        }

        public SQLContex(ApplicationDbContext context, Guid userId)
        {
            _context = context;
            StreamerId = userId;
        }

        public SQLContex(ApplicationDbContext context, string streamerId)
        {
            _context = context;


            StreamerId = _context.Accounts.FirstOrDefault(x => x.Username == streamerId)?.Id ?? new Guid();
        }

        public async Task<List<Viewer>> GetLoyalityAsync()
        {
            //.Where(x=>x.User.Id == UserId)
            var q = await _context.Viewers.Include(x=>x.Wallets).ToListAsync();
            return q;
        }

        public async Task<List<Viewer>> GetActiveLoyalityAsync()
        {
            var when = DateTime.Now.AddMinutes(-15);
            var q = await _context.Viewers.Include(x => x.Wallets).Where(x=>x.LastActive > when).ToListAsync();
            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ammount"></param>
        /// <returns>success</returns>


        public string GetAccountID()
        {
            return StreamerId.ToString();
        }

        public async Task<int> AddPointToAllViewersAndCountAsync(int ammount)
        {
            int count = 0;
            var usersLoyalty = await GetActiveLoyalityAsync();
            
            var userLoyaltyAux = usersLoyalty;
            foreach (var user in userLoyaltyAux)
            {
                if(user.MemberLevel > MemberLevels.Level1)
                {
                    var existingChild = user;
                    count++;
                    existingChild.Wallets.FirstOrDefault(x => x.StreamerId == StreamerId.ToString()).Coins += ammount + (int)(ammount * 0.5);
                    _context.Entry(user).CurrentValues.SetValues(existingChild);
                }
                else
                {
                    var existingChild = user;
                    count++;
                    existingChild.Wallets.FirstOrDefault(x => x.StreamerId == StreamerId.ToString()).Coins += ammount;
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
            return await _context.Giveways.Where(x=>x.OwnerId == StreamerId.ToString()).ToListAsync();
        }

        public async Task<List<GivewayTiket>> GetGivewayTikets()
        {
            return await _context.GivewayTikets.ToListAsync();
        }

        public async Task<ViewerWallet> CreateUserWallet(string viewerId)
        {
            var viewerWallet = new ViewerWallet()
            {
                Coins = 0,
                StreamerId = GetAccountID(),
                ViewerId = viewerId            
            };

            await _context.ViewerWallets.AddAsync(viewerWallet);

            return await _context.SaveChangesAsync() > 0 ? viewerWallet : null;
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

            List<string> giveawayWinners = new List<string>();

            await Task.Run(async () =>
            {
                while (giveawayWinners.Count() < giveaway.WinnersCount)
                {
                    var winnerNumber = 0;
                    var winner = "";
                    
                    winnerNumber = new Random().Next(0, thisGWTikets.Count());
                    winner = thisGWTikets[winnerNumber].ViewerID;
                    thisGWTikets.RemoveAll(x => x.ViewerID == winner);
                    if (giveawayWinners.Any(x => x == winner) == false)
                    {
                        giveawayWinners.Add(winner);
                    }

                    if (giveawayWinners.Count() == giveaway.WinnersCount || thisGWTikets.Count <= 0)
                    {
                        break;
                    }

                    await Task.Delay(5);
                }
            });

            Random rng = new();
            var giveawayWinnersShufle = giveawayWinners.OrderBy(a => rng.Next()).ToList();
            foreach (var winner in giveawayWinnersShufle)
            {
                await _context.Winners.AddAsync(new Winner() { GiveawayID = id, ViewerID = winner });
            }

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
            
            return await _context.Viewers.Include(x => x.Wallets).FirstOrDefaultAsync(x => x.Id == userID);
        }

        public async Task<bool> AddPointToViewerAsync(Viewer viewer, int ammount, bool doubleUp = false)
        {
            if (doubleUp == true)
            {
                if (viewer.MemberLevel > MemberLevels.Level1)
                {
                    ammount += (int)(ammount * 0.5);
                }
            }

            viewer.Wallets.FirstOrDefault(x => x.StreamerId == StreamerId.ToString()).Coins += ammount;
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

        public async Task<bool> SetUserLevel(string id, MemberLevels level, DateTime expires)
        {
            var viewer = await GetViewerModel(id);
            viewer.MemberLevel = level;
            viewer.ExpiresMember = expires;
            return await SaveUser(viewer);
        }

        public async Task<ViewerWallet> GetViewerWallet(string viewerId)
        {
            var viewer = await GetViewerModel(viewerId);
            if (viewer.Wallets.Any(x => x.StreamerId == GetAccountID()))
            {
                return viewer.Wallets.FirstOrDefault(x => x.StreamerId == GetAccountID());
            }

           return await CreateUserWallet(viewerId);
        }
        public async Task<bool> SetYoutubeToken(string youtubetoken)
        {
            (await _context.Accounts.FirstOrDefaultAsync(x => x.Id == StreamerId)).YoutubeToken = youtubetoken;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetTwitchToken(string youtubetoken,string twitchId)
        {
            (await _context.Accounts.FirstOrDefaultAsync(x=>x.Id == StreamerId)).TwitchToken = youtubetoken;
            (await _context.Accounts.FirstOrDefaultAsync(x=>x.Id == StreamerId)).TwitchId = twitchId;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<string> GetYoutubeToken()
        {
           return (await _context.Accounts.FirstOrDefaultAsync(x => x.Id == StreamerId)).YoutubeToken;
        }

        public async Task<string> GetTwitchToken()
        {
            return (await _context.Accounts.FirstOrDefaultAsync(x => x.Id == StreamerId)).TwitchToken;
        }

        public async Task<bool> RemovePointsToOneUser(Viewer viewer, int ammount, bool doubleUp)
        {
            if (viewer.Wallets?.FirstOrDefault(x => x.StreamerId == StreamerId.ToString())?.Coins < ammount)
            {
                return false;
            }
            else
            {
                viewer.Wallets.FirstOrDefault(x => x.StreamerId == StreamerId.ToString()).Coins -= ammount;
                return await SaveUser(viewer);
            }
        }

        public Guid GetAccountGuID()
        {
            return StreamerId;
        }
    }
}
