using DatabaseContext;
using DatabaseContextCore;
using DataLayer;
using Dynamitey.DynamicObjects;
using JWTManager;
using LocalDatabseManager;
using MemoryManipulator;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Settings;
using SQLContextManager;
using StaticDatabase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Youtube_Contractor;
using static Google.Apis.Requests.BatchRequest;

namespace LocalDatabaseManager
{
    public class Database
    {
        

        DatabaseReadWrite UnmanagedFileModifier;
        MemoryManager MemoryManager;
        public SQLContex SQLContextManager;
        public ValidationModel ValidationResponse { get; set; }
        

        public Database(string username = "", SQLContex contex = null)
        {
            UnmanagedFileModifier = new DatabaseReadWrite(username);
            MemoryManager = new MemoryManager(username);
            SQLContextManager = contex;
        }

        public Guid GetAccountGuID()
        {
            return SQLContextManager.GetAccountGuID();
        }

        public string GetAccountID()
        {
            return SQLContextManager.GetAccountID();
        }

        public ClasamentPacaniada GetClasamentPacaniada()
        {
            return MemoryManager.GetPacaniada();
        }

        public bool SetClasamentPacaniada(ClasamentPacaniada clasamentPacaniada)
        {
            return MemoryManager.SetPacaniada(clasamentPacaniada);
        }

        public Database LoginUserReadOnlyAsync(string streamerId, ApplicationDbContext dbContext)
        {
            UnmanagedFileModifier = new DatabaseReadWrite(streamerId);
            MemoryManager = new MemoryManager(streamerId);
            SQLContextManager = new SQLContex(dbContext, streamerId);
            return this;
        }

        public async Task<Database> LoginUserAsync(string token, ApplicationDbContext dbContext)
        {
            ValidationResponse = await JwtManager.ValidateTokenAsync(token, dbContext);
            if (ValidationResponse.ValidationResponse == JWTManager.ValidationResponse.Success)
            {
                UnmanagedFileModifier = new DatabaseReadWrite(ValidationResponse.UserName);
                MemoryManager = new MemoryManager(ValidationResponse.UserName);
                SQLContextManager = new SQLContex(dbContext, ValidationResponse.UserGuid);
            }

            return this;
        }

        public bool SaveShop(List<ShopItem> shopItems)
        {
            return MemoryManager.SaveShop(shopItems);
        }

        public async Task RemoveBroadcastMessageAsync(Viewer viewer)
        {
            await SQLContextManager.RemoveBroadcastMessageAsync(viewer);
        }

        public void BroadcastMessage(string userName, string message)
        {
            MemoryManager.BroadcastMessage(userName, message);
        }

        public async Task<List<GivewayModel>> GetGivewayListAsync()
        {
            return await SQLContextManager.GetGiveawayList();
        }

        public async Task<List<GivewayTiket>> GetGivewayTikets()
        {
            return await SQLContextManager.GetGivewayTikets();
        }

        public async Task<List<ShopItem>> GetShopAsync()
        {
            var shop = UnmanagedFileModifier.ReadFile<List<ShopItem>>(ProjectSettings.Shop);

            return shop;
        }

        public void UpdateBetting(BettingModel bettingModel)
        {
            MemoryManager.SaveLiveBetting(bettingModel);
        }

        public async Task<string> BuyGiveawayTiket(Viewer viewer, int gid)
        {
            GivewayModel giveway = await SQLContextManager.GetGivewayModel(gid);

            var userTikets = await SQLContextManager.GetViewerGivewayTokens(viewer.Id, giveway.Id);

            if (userTikets >= giveway.MaxTikets)
            {
                return $"You can`t buy more than {giveway.MaxTikets} tikets for this giveaway.";
            }
            if (await SQLContextManager.RemovePointsToOneUser(viewer, giveway.Price, false))
            {
                if (await SQLContextManager.CreateGivewayTiket(viewer, giveway))
                {
                    return $"You bought a token!";
                }
                else
                {
                    if (await SQLContextManager.AddPointToViewerAsync(viewer, giveway.Price, false))
                    {
                        return $"Error buying token but you have not been charged!";
                    }
                }
            }
        

            return "Error";
        }

        public async Task<string> ValidateUser(string accountId)
        {
            return await SQLContextManager.ValidateUser(accountId);
        }

        public async Task<List<Viewer>> GetLoyaltyPointsAsync()
        {
            return await SQLContextManager.GetLoyalityAsync();
        }

        public InPlayGame GetInPlayGame()
        {
            return UnmanagedFileModifier.ReadFile<InPlayGame>(ProjectSettings.InPlayGame);
        }

        public string GetCustomTheme()
        {
            return UnmanagedFileModifier.ReadString(ProjectSettings.CustomThemeFile);
        }

        public async Task<string> SetUserPrecence(string viewerID, string viewerName)
        {
            return await SQLContextManager.SetUserActive(viewerID, viewerName);
        }

        public async Task<Viewer> GetViewerAsync(string viewerID)
        {
            return await SQLContextManager.GetViewerModel(viewerID) ?? null;
        }

        public async Task<string> WinPointsAsync(Viewer utilizator, int ammount)
        {
            await new YoutubeChatWriter().WriteMessageAsync($"@{utilizator.Name} tocmai a castigat {ammount} {ProjectSettings.NumePuncteLoialitate} pe !shop.");
            return await SQLContextManager.AddPointToViewerAsync(utilizator, ammount, false) ? $"Ai primit {ammount} de {ProjectSettings.NumePuncteLoialitate}!" : $"Din pacate nu a reusit transferul a {ammount}";
        }

        public List<LigaUser> GetLiga()
        {
            return MemoryManager.GetLiga();
        }

        public bool AddLiga(LigaUser ligaUser)
        {
            return MemoryManager.AddLiga(ligaUser);
        }

        public string RedeemItem(Viewer user, ShopItem item, string additionaldata = "")
        {

            Redeem redeem = new()
            {
                UserId = user.Id,
                ShopItem = item,
                //ViewerSettings = MemoryManager.GetUserSettingsForStreamerPage(user.Id),
                WinnerName = user.Name
            };
            redeem.ShopItem.OptionalData = additionaldata;
            MemoryManager.SaveRedeem(redeem);

            var shop = UnmanagedFileModifier.ReadFile<List<ShopItem>>(ProjectSettings.Shop);
            shop.FirstOrDefault(x => x.ItemID == item.ItemID && x.Nume == item.Nume).Stoc -= 1;
            UnmanagedFileModifier.WriteFile(ProjectSettings.Shop, JsonConvert.SerializeObject(shop));

            if (string.IsNullOrWhiteSpace(additionaldata))
            {
                return $"Ai cumparat cu success {item.Nume}";
            }
            else
            {
                return $"Ai cumparat cu success {item.Nume} si ai primit {additionaldata}";
            }
        }

        public string sGetUserCooldown(string id, string cmd)
        {
            var cd = UnmanagedFileModifier.ReadFile<List<UserCooldown>>(ProjectSettings.CooldownFolder + cmd + ProjectSettings.CooldownFile) ?? new List<UserCooldown>();
            return cd.Where(x => x.userID == id && x.Expires > DateTime.Now).FirstOrDefault().Expires.ToString("yyyy/MM/dd HH:mm:ss");
        }

        public double dGetUserCooldown(string id, string cmd)
        {
            if (!IsUserOnCooldown(id, cmd))
            {
                return 0;
            }

            var cd = UnmanagedFileModifier.ReadFile<List<UserCooldown>>(ProjectSettings.CooldownFolder + cmd + ProjectSettings.CooldownFile) ?? new List<UserCooldown>();

            return (cd.Where(x => x.userID == id && x.Expires > DateTime.Now).FirstOrDefault().Expires - DateTime.Now).TotalMinutes;
        }

        public async Task<List<Viewer>> GetViewerByNameAsync(string userName)
        {
            return await SQLContextManager.GetViewerByNameAsync(userName);
        }

        public async Task<bool> RemovePointsToOneUser(Viewer viewer, int ammount, bool doubleUp = true)
        {
            return await SQLContextManager.RemovePointsToOneUser(viewer, ammount, doubleUp);
        }

        public async Task<bool> AddPointsToOneUser(Viewer viewer, int ammount, bool doubleUp = false)
        {
            viewer.LastActive = DateTime.Now;
            return await SQLContextManager.AddPointToViewerAsync(viewer, ammount, doubleUp);
        }

        public async Task<int> AddPointsAllAsync(int ammount)
        {
            return await SQLContextManager.AddPointToAllViewersAndCountAsync(ammount);
        }

        public async Task<bool> SetMemberLevel(string id, string slevel, DateTime expires)
        {
            var level = GetLevel(slevel);
            return await SQLContextManager.SetUserLevel(id, level, expires.AddDays(31));
        }

        private MemberLevels GetLevel(string slevel)
        {
            return MemoryManager.GetMemberLevels(slevel);
        }

        private async Task<bool> SaveLoyaltyAsync(Viewer viewer)
        {
            return await SQLContextManager.SaveUser(viewer);
        }

        public bool StergeUserDinLiga(LigaUser user)
        {
            return MemoryManager.StergeUserDinLiga(user);
        }

        public int RedeemOfItem(string itemID)
        {
            string file = string.Format(ProjectSettings.RedeemsFile, itemID);
            var shop = UnmanagedFileModifier.ReadString(file);

            var shCount = shop.Split("\r\n");

            return shCount.Count();
        }

        public BettingModel GetLiveBetting()
        {
            return MemoryManager.GetLiveBeting();
        }

        public Tranzactii GetTranzactii()
        {
            return UnmanagedFileModifier.ReadFile<Tranzactii>(ProjectSettings.TranzactiiFile);
        }

        public void SetCustomTheme(string customTheme)
        {
            string file = ProjectSettings.CustomThemeFile;
            UnmanagedFileModifier.WriteFile(file, customTheme);
        }

        public void CreateBettingFromBonusHunt(int maxBet)
        {
            BettingModel BettingModel = new BettingModel();
            BettingModel.MaxBet = maxBet;
            BettingModel.VoteTitle = "Payout BonusHunt?";
            var bh = UnmanagedFileModifier.ReadFile<BonusHuntFullInfo>(ProjectSettings.LiveBonusHuntFile);
            var bhval = bh.HuntValue;
            var step = (decimal)bhval / 10;


            string opt1 = "< " + step * 7 / 1000 + "k";
            string middleOpt = "{0}k-{1}k";
            string lastOpt = "> " + step * 13 / 1000 + "k";

            BettingModel.Options.Add(new BettingOptionModel(0, maxBet)
            {
                Nume = opt1
            });

            for (int i = 1; i < 7; i++)
            {
                BettingModel.Options.Add(new BettingOptionModel(i, maxBet)
                {
                    Nume = string.Format(middleOpt, step * (6 + i) / 1000, step * (7 + i) / 1000)
                });
            }

            BettingModel.Options.Add(new BettingOptionModel(7, maxBet)
            {
                Nume = lastOpt
            });

            DeleteAllColldown("bet");
            SetLiveBetting(BettingModel);
        }

        private void DeleteAllColldown(string v)
        {
            MemoryManager.DeleteAllColldown(v);
        }

        public void TestFile(string v, string toAppend)
        {
            UnmanagedFileModifier.WriteFile(v, toAppend);
        }

        private string SaveHotWords(List<HotWord> hotWords)
        {
            string file = ProjectSettings.HotWords;
            return MemoryManager.SaveHotWords(file, hotWords);
        }

        public void AddSingleGameToBH(InPlayGame gameName, double betSize)
        {
            var bonushunt = GetLiveBonusHunt();
            if (bonushunt != null)
            {
                if (bonushunt.Bonuses.Any(x => x.GameName == gameName.Game.Name))
                {
                    bonushunt.Bonuses.FirstOrDefault(x=>x.GameName == gameName.Game.Name).BetSize = betSize;
                }
                else
                {
                    bonushunt.Bonuses.Add(new BonusHuntGridObject() { BetSize = betSize, PlayerName = gameName.PlayerName, GameName = gameName.Game.Name, Payed = 0, IsCurrent = false, ProviderName = gameName.Game.Provider });
                }
            }

            UpdateBonusHunt(bonushunt);
        }

        public string TestFileRead(string v)
        {
            return UnmanagedFileModifier.ReadString(v);
        }

        public async Task<string> RegisterNewViewerFromChat(string userID, string userName, int ammount, string cmd = "")
        {
            if (await SQLContextManager.ViewerExist(userID))
            {
                Viewer viewer = await SQLContextManager.GetViewerModel(userID);
                if (viewer.Name != userName)
                {
                    viewer.Name = userName;
                }
                return string.Format("@{0} Esti inscris in sistemul de loialitate!", userName, ammount);
            }
            else
            {
                return await CreateUserFromScrapAsync(userID, userName, ammount);
            }
        }

        public bool IsUserOnCooldown(string userID, string cmd)
        {
            var cd = UnmanagedFileModifier.ReadFile<List<UserCooldown>>(ProjectSettings.CooldownFolder + cmd + ProjectSettings.CooldownFile) ?? new List<UserCooldown>();
            if (cd.Any(x => x.userID == userID && x.Expires > DateTime.Now) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Add User On Cooldown after completing a certain action!
        /// </summary>
        /// <param name="userID">user to be put on cooldown</param>
        /// <param name="cmd">Cooldown cmd</param>
        /// <param name="cooldownMinutes">Colldown in minutes ex 0.1 1.5 etc...</param>
        public void AddUserOnCooldown(string userID, string cmd, double cooldownMinutes = 5)
        {
            var cd = UnmanagedFileModifier.ReadFile<List<UserCooldown>>(ProjectSettings.CooldownFolder + cmd + ProjectSettings.CooldownFile) ?? new List<UserCooldown>();
            if (cd.Any(x => x.userID == userID))
            {
                cd.RemoveAll(x => x.userID == userID);
            }

            cd.Add(new UserCooldown() { userID = userID, Expires = DateTime.Now.AddMinutes(cooldownMinutes) });

            MemoryManager.PutUserOnCooldown(cd, cmd);
        }

        public BonusHuntFullInfo GetLiveBonusHunt()
        {
            return UnmanagedFileModifier.ReadFile<BonusHuntFullInfo>(ProjectSettings.LiveBonusHuntFile);
        }

        public void SetLiveBetting(BettingModel bettingModel)
        {
            MemoryManager.SaveLiveBetting(bettingModel);
        }        

        public bool SetInplayGame(InPlayGame inPlayGame)
        {
            var curent = GetInPlayGame();

            var inDbGame = GetGame(inPlayGame.Game.Name);
            var bonushunt = GetLiveBonusHunt();

            if (curent.Game.Name == inPlayGame.Game.Name || inDbGame == null)
            {
                UnmanagedFileModifier.SaveNewGame(inPlayGame);
            }


            if (inDbGame != null)
                SaveInPlayGame(new InPlayGame() { PlayerName = inPlayGame.PlayerName, Game = inDbGame, InHuntNumber = bonushunt.Bonuses.Where(x => x.Payed > 0).Count() + "/" + bonushunt.Bonuses.Count() });
            else
                SaveInPlayGame(new InPlayGame() { PlayerName = inPlayGame.PlayerName, Game = new Game() { Name = inPlayGame.Game.Name }, InHuntNumber = bonushunt.Bonuses.Where(x => x.Payed > 0).Count() + "/" + bonushunt.Bonuses.Count() });
            return true;
        }

        public async Task<string> SetBettingWinnerAsync(string bettingOption)
        {
            var loyalityRanking = await SQLContextManager.GetLoyalityAsync();
            int jackpotAmmount = await SQLContextManager.ReadUserJackpot();
            string file = ProjectSettings.LiveBettingUserOptions + bettingOption + ".json";
            List<UserBet> userBets = MemoryManager.GetUserBets(bettingOption);

            BettingModel bettingModels = UnmanagedFileModifier.ReadFile<BettingModel>(ProjectSettings.LiveBetting);

            bool japotWon = false;


            int totalammount = 0;
            foreach (var bet in bettingModels.Options)
            {
                //if (japotWon == false)
                //{
                //    int p = new Random().Next(0, int.MaxValue);
                //    if (p < 2006)
                //    {
                //        var jkwinner = loyalityRanking.Where(x => x.Username == bet.Nume).FirstOrDefault();
                //        if (jkwinner != null)
                //        {
                //            japotWon = true;
                //            jackpotWinner = jkwinner;
                //        }
                //    }
                //}
                var opts = MemoryManager.GetUserBets(bet.Key);
                var vvv = opts?.Sum(x => x.Bet) ?? 0;
                totalammount += vvv;
            }

            var ammountToRecieve = 1 + (totalammount / userBets.Sum(x => x.Bet));

            var totalSpent = 0;

            foreach (var user in userBets)
            {
                var winner = loyalityRanking?.Where(x => x.Id == user.UserID)?.FirstOrDefault();
                if (winner != null)
                {
                    await SQLContextManager.AddPointToViewerAsync(winner, user.Bet * ammountToRecieve);
                    totalSpent += user.Bet * ammountToRecieve;
                }
                else
                {
                    //LOG USER INVALID

                }
            }

            UnmanagedFileModifier.RemoveBettingFiles();

            return "Option -" + bettingOption + "- won the pool," +
            "all betters recieved " + ammountToRecieve + " for each point they " +
            "bet! In a total of " + totalSpent + " spread to " + userBets.Count + " winners. ";
        }

        private void EndBetting(bool jackpotWon)
        {
            UnmanagedFileModifier.RemoveBettingFiles();
            if (jackpotWon)
            {
                UnmanagedFileModifier.WriteFile(Settings.ProjectSettings.JackpotFile, new Random().Next(1200, 6000).ToString());
            }
            else
            {
                int jackpotAmmount = UnmanagedFileModifier.ReadUserJackpot();
                UnmanagedFileModifier.WriteFile(Settings.ProjectSettings.JackpotFile, new Random().Next(jackpotAmmount + 1200, jackpotAmmount + 4000).ToString());
            }
        }

        public async Task<string> IncrementBettingOptionAsync(string userID, string key, int ammount)
        {

            if (IsUserOnCooldown(userID, "bet"))
            {
                return " you can`t vote twice!";
            }

            var viewer = await GetViewerAsync(userID);
            if (await RemovePointsToOneUser(viewer, ammount))
            {
                var betting = GetLiveBetting();

                var opt = betting.Options.Where(x => x.Key == key)?.FirstOrDefault();

                if (opt != null)
                {
                    opt.Voturi++;
                    opt.TotalPariat += ammount;
                }
                else
                {
                    return " invalid betting option! try one of the following :" + betting.Options.Where(x=>x.IsVisible == true).Select(x => x.Key).Aggregate((a, b) => a + ", " + b);
                }
                SetLiveBetting(betting);
                AddUserOnCooldown(userID, "bet", 30);
                string response = SaveUsersBets(userID: userID, key: key, amount: ammount);

                return response;
            }
            else
            {                
                return $" you have only {(await GetViewerWallet(userID)).Coins} {GetStreamerSettings().LoyaltySettings.LoyaltyName}";
            }
        }

        public async Task<bool> CreateUser(string userID, string name = "", string googleEmail = "", string ipAdress = "")
        {
            Viewer viewer = new Viewer()
            {
                CreationTime = DateTime.Now,
                Id = userID,
                Name = name,
                LastActive = DateTime.Now,
                ExpiresMember = DateTime.Now,
                Ipadress = ipAdress,
                IsActive = false,
                BroadcastMessageCount = 0,
                Email = googleEmail,
                MemberLevel = MemberLevels.Viewer,
                SuperbetName = ""

            };
            var creationStatus = await SQLContextManager.CreateUser(viewer) && await SQLContextManager.CreateUserWallet(viewer.Id) != null;

            return creationStatus;
        }

        private async Task<string> CreateUserFromScrapAsync(string userID, string userName, int ammount = 10)
        {
            Viewer viewer = new Viewer()
            {
                CreationTime = DateTime.Now,
                Id = userID,
                Name = userName,
                LastActive = DateTime.Now,
                ExpiresMember = DateTime.Now,
                Ipadress = "",
                IsActive = false,
                BroadcastMessageCount = 0,
                Email = "",
                MemberLevel = MemberLevels.Viewer,
                SuperbetName = ""

            };

            if (await SQLContextManager.CreateUser(viewer))
            {
                return string.Format("@{0} te-ai inscris in sistemul de loyalitate si ai primit 10 {1}, nu uita sa te inscrii si pe https://coxino.ro/shop , acum poti sa votezi!", userName, ProjectSettings.NumePuncteLoialitate);
            }
            else
            {
                Viewer viewer2 = new Viewer()
                {
                    CreationTime = DateTime.Now,
                    Id = userID,
                    Name = userName.ToAscii(),
                    LastActive = DateTime.Now,
                    ExpiresMember = DateTime.Now,
                    IsActive = false,
                    BroadcastMessageCount = 0,
                    Email = "",
                    Ipadress = "",
                    MemberLevel = MemberLevels.Viewer,
                    SuperbetName = ""
                };
                if (await SQLContextManager.CreateUser(viewer2))
                {
                    return string.Format("@{0} te-ai inscris in sistemul de loyalitate si ai primit 10 {1} dar din cauza caracterelor speciale din numele tau, este posibil ca unele functii sa nu mearga corespunzator. numele tau in sistem va fi : {2}", userName, ProjectSettings.NumePuncteLoialitate, viewer2.Name);
                }
                else
                {
                    return string.Format("@{0} nu am putut sa te inscriu in sistemul de loyalitate din cauza caracterelor speciale din numele tau, schimba numele sau incearca direct pe site! https://coxino.ro/shop", userName);
                }
            }
        }

        private string SaveUsersBets(string userID, string key, int amount)
        {
            string file = ProjectSettings.LiveBettingUserOptions + key + ".json";
            return MemoryManager.SaveUserBets(file, viewerID: userID, amount, key);
        }

        public void CreateBettingFromTournament(int maxBet)
        {
            BettingModel BettingModel = new BettingModel();
            BettingModel.MaxBet = maxBet;
            BettingModel.VoteTitle = "Vote your favourite!";
            List<string> allNames = GetLiveTournament().MeciuriSferturi.Select(x => x.Team1.Nume).ToList();
            allNames.AddRange(GetLiveTournament().MeciuriSferturi.Select(x => x.Team2.Nume).ToList());
            for (int i = 0; i < 8; i++)
            {
                BettingModel.Options.Add(new BettingOptionModel(i, maxBet)
                {
                    Nume = allNames[i],
                });
            }

            DeleteAllColldown("bet");
            SetLiveBetting(BettingModel);
        }

        public void UpdateTournament(Meci meci)
        {
            var turneu = GetLiveTournament();

            var meci_curent = turneu.MeciuriOptimi.FirstOrDefault(x => x.Team1.HasWon == false && x.Team2.HasWon == false);

            if (meci_curent == null || turneu.IsOptimi == false)
                meci_curent = turneu.MeciuriSferturi.FirstOrDefault(x => x.Team1.HasWon == false && x.Team2.HasWon == false);
            if (meci_curent == null)
                meci_curent = turneu.MeciuriSemiFinale.FirstOrDefault(x => x.Team1.HasWon == false && x.Team2.HasWon == false);
            if (meci_curent == null)
                meci_curent = turneu.MeciFinal;

            if (meci_curent != null)
            {
                meci_curent.Team1 = meci.Team1;
                meci_curent.Team2 = meci.Team2;
            }

            UpdateTournament(turneu);
            SaveCurrentTournamentLiveGame(meci);
        }

        public bool SetInplayGame(string inPlayGame)
        {
            var inDbGame = GetGame(inPlayGame);

            var InPlay = new InPlayGame()
            {
                Game = inDbGame,
                InHuntNumber = "",
            };

            return SetInplayGame(InPlay);
        }

        public Meci GetLiveTournamentLiveFight()
        {
            return UnmanagedFileModifier.ReadFile<Meci>(ProjectSettings.TournamentLiveGameFile);
        }

        public void SetTranzactii(Tranzactii tranzactii)
        {
            SaveTranzactii(tranzactii);
        }

        public Tournament GetLiveTournament()
        {
            return UnmanagedFileModifier.ReadFile<Tournament>(ProjectSettings.TournamentFile);
        }

        public void SetCustomTheme()
        {
            throw new NotImplementedException();
        }

        public void UpdateBonusHunt(BonusHuntFullInfo bonusHunt)
        {
            if (!bonusHunt.Bonuses.Any(x => x.BetSize <= 0))
            {
                bonusHunt.Bonuses = bonusHunt.Bonuses.OrderBy(x => x.BetSize).ToList();
            }

            foreach (var game in bonusHunt.Bonuses)
            {
                game.IsCurrent = false;
            }

            SaveCurrentBonusHunt(bonusHunt);
        }

        public BonusHuntPreInfo getInfoFromBH(BonusHuntFullInfo bonusHunt)
        {
            BonusHuntPreInfo bonusHuntPreInfo = new();
            try
            {
                bonusHuntPreInfo.AverageBet = (bonusHunt?.Bonuses?.Average(x => x.BetSize) ?? 0).ToString("0.##");
                bonusHuntPreInfo.TotalLoss = ((bonusHunt?.HuntValue - bonusHunt?.Bonuses?.Sum(x => x.Payed)) ?? 0).ToString("### ###");
                bonusHuntPreInfo.BestPayValue = (bonusHunt?.Bonuses?.Max(x => x.Payed) ?? 0).ToString("0.##");
                bonusHuntPreInfo.Hunting = bonusHunt?.IsHunting ?? false;
                bonusHuntPreInfo.AverageMultiToBreakEven = (((bonusHunt?.HuntValue - bonusHunt?.Bonuses?.Sum(x => x.Payed)) / bonusHunt?.Bonuses?.Where(x => x.Payed > 0)?.Count() / bonusHunt?.Bonuses?.Where(x => x.Payed > 0).Average(x => x.BetSize)) ?? 0).ToString("### ###");
                bonusHuntPreInfo.AverageMulti = (bonusHunt?.Bonuses?.Where(x => x.Payed > 0)?.Average(x => x.Multiplier) ?? 0).ToString("0.##");
                bonusHuntPreInfo.AverageWin = (bonusHunt?.Bonuses?.Where(x => x.Payed > 0)?.Average(x => x.Payed) ?? 0).ToString("0.##");
            }
            catch
            {

            }
            return bonusHuntPreInfo;
        }

        private void SaveInPlayGame(BonusHuntGridObject inplay, BonusHuntPreInfo bonusHuntPreInfo, int cnt = 0, int total = 0)
        {

            InPlayGame inPlay = new InPlayGame()
            {
                Game = GetGame(inplay.GameName),
                InHuntNumber = cnt + "/" + total
            };

            inPlay.PlayerName = inplay.PlayerName;

            SaveInPlayGame(inPlay);
        }

        private Game GetGame(string gameName, bool withRounds = true)
        {
            var game = UnmanagedFileModifier.GetGame(gameName,withRounds) ?? new Game() { Name = gameName };

            return game;
        }


        public void UpdateTournamentLiveFight(string json)
        {
            string file = ProjectSettings.TournamentLiveGameFile;
            UnmanagedFileModifier.WriteFile(file, json);
        }

        public void DeleteLiveBonus(string bonus)
        {
            var bonusEntry = GetLiveBonusHunt();
            var toRemove = bonusEntry.Bonuses.FirstOrDefault(x => x.GameName == bonus);
            bonusEntry.Bonuses.Remove(toRemove);

            SaveCurrentBonusHunt(bonusEntry);
        }

        public void DeleteLiveBonus(int no)
        {
            var bonusEntry = GetLiveBonusHunt();
            var toRemove = bonusEntry.Bonuses[no];
            bonusEntry.Bonuses.Remove(toRemove);

            SaveCurrentBonusHunt(bonusEntry);
        }

        public void CreateTournament(TournamentCreateInfos json)
        {
            Tournament t = new Tournament(json);

            string file = ProjectSettings.TournamentFile;
            UnmanagedFileModifier.WriteFile(file, JsonConvert.SerializeObject(t));
        }

        public void CloseCurrentFight()
        {
            CloseCurrentTournamentLiveFight();
        }

        private void CloseCurrentTournamentLiveFight()
        {
            var LiveTournament = GetLiveTournament();
            var aux = GetLiveTournamentLiveFight();
            Meci TournamentLiveGame = null;

            if (LiveTournament.IsOptimi)
            {
                TournamentLiveGame = LiveTournament.MeciuriOptimi.Where(x => x.Team1.Nume == aux?.Team1?.Nume && x.Team2.Nume == aux?.Team2?.Nume).FirstOrDefault() ?? LiveTournament.MeciuriOptimi.Where(x => x.Team1.HasWon == false && x.Team2.HasWon == false).FirstOrDefault();
            }
            if (TournamentLiveGame == null)
            {
                TournamentLiveGame = LiveTournament.MeciuriSferturi.Where(x => x.Team1.Nume == aux.Team1.Nume && x.Team2.Nume == aux.Team2.Nume).FirstOrDefault();
            }
            if (TournamentLiveGame == null)
            {
                TournamentLiveGame = LiveTournament.MeciuriSemiFinale.Where(x => x.Team1.Nume == aux.Team1.Nume && x.Team2.Nume == aux.Team2.Nume).FirstOrDefault();
            }
            if (TournamentLiveGame == null)
            {
                TournamentLiveGame = LiveTournament.MeciFinal;
            }

            if (TournamentLiveGame.Team1.Scor > TournamentLiveGame.Team2.Scor)
            {
                TournamentLiveGame.Team1.HasWon = true;
                TournamentLiveGame.Team1.PrevX = TournamentLiveGame.Team1.Scor;

                if (LiveTournament.IsOptimi == true)
                {
                    var nextBattle = LiveTournament.MeciuriSferturi
                         .Where(x => string.IsNullOrWhiteSpace(x.Team1.Nume) || string.IsNullOrWhiteSpace(x.Team2.Nume)).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(nextBattle.Team1.Nume))
                    {
                        nextBattle.Team1 = new TeamStats(TournamentLiveGame.Team1);
                    }

                    else if (string.IsNullOrWhiteSpace(nextBattle.Team2.Nume))
                    {
                        nextBattle.Team2 = new TeamStats(TournamentLiveGame.Team1);
                    }
                }

                if (LiveTournament.IsQuarter == true && LiveTournament.IsOptimi == false)
                {
                    var nextBattle = LiveTournament.MeciuriSemiFinale
                         .Where(x => string.IsNullOrWhiteSpace(x.Team1.Nume) || string.IsNullOrWhiteSpace(x.Team2.Nume)).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(nextBattle.Team1.Nume))
                    {
                        nextBattle.Team1 = new TeamStats(TournamentLiveGame.Team1);
                    }

                    else if (string.IsNullOrWhiteSpace(nextBattle.Team2.Nume))
                    {
                        nextBattle.Team2 = new TeamStats(TournamentLiveGame.Team1);
                    }
                }

                if (LiveTournament.IsSemis == true && LiveTournament.IsQuarter == false)
                {
                    if (string.IsNullOrWhiteSpace(LiveTournament.MeciFinal.Team1.Nume))
                    {
                        LiveTournament.MeciFinal.Team1 = new TeamStats(TournamentLiveGame.Team1);
                    }

                    else if (string.IsNullOrWhiteSpace(LiveTournament.MeciFinal.Team2.Nume))
                    {
                        LiveTournament.MeciFinal.Team2 = new TeamStats(TournamentLiveGame.Team1);
                    }
                }
            }
            else
            {
                TournamentLiveGame.Team2.HasWon = true;
                TournamentLiveGame.Team2.PrevX = TournamentLiveGame.Team2.Scor;

                if (LiveTournament.IsOptimi == true)
                {

                    var nextBattle = LiveTournament.MeciuriSferturi
                         .Where(x => string.IsNullOrWhiteSpace(x.Team1.Nume) || string.IsNullOrWhiteSpace(x.Team2.Nume)).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(nextBattle.Team1.Nume))
                    {
                        nextBattle.Team1 = new TeamStats(TournamentLiveGame.Team2);
                    }

                    else if (string.IsNullOrWhiteSpace(nextBattle.Team2.Nume))
                    {
                        nextBattle.Team2 = new TeamStats(TournamentLiveGame.Team2);
                    }
                }

                if (LiveTournament.IsQuarter == true && LiveTournament.IsOptimi == false)
                {

                    var nextBattle = LiveTournament.MeciuriSemiFinale
                         .Where(x => string.IsNullOrWhiteSpace(x.Team1.Nume) || string.IsNullOrWhiteSpace(x.Team2.Nume)).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(nextBattle.Team1.Nume))
                    {
                        nextBattle.Team1 = new TeamStats(TournamentLiveGame.Team2);
                    }

                    else if (string.IsNullOrWhiteSpace(nextBattle.Team2.Nume))
                    {
                        nextBattle.Team2 = new TeamStats(TournamentLiveGame.Team2);
                    }
                }

                if (LiveTournament.IsSemis == true && LiveTournament.IsQuarter == false)
                {
                    if (string.IsNullOrWhiteSpace(LiveTournament.MeciFinal.Team1.Nume))
                    {
                        LiveTournament.MeciFinal.Team1 = new TeamStats(TournamentLiveGame.Team2);
                    }

                    else if (string.IsNullOrWhiteSpace(LiveTournament.MeciFinal.Team2.Nume))
                    {
                        LiveTournament.MeciFinal.Team2 = new TeamStats(TournamentLiveGame.Team2);
                    }
                }
            }

            if (LiveTournament.IsOptimi == true)
            {
                TournamentLiveGame = LiveTournament.MeciuriOptimi.Where(x => x.Team1.HasWon == false && x.Team2.HasWon == false).FirstOrDefault();
                if (TournamentLiveGame == null)
                {
                    LiveTournament.IsOptimi = false;
                    LiveTournament.IsQuarter = true;
                }
            }


            if (LiveTournament.IsQuarter == true && LiveTournament.IsOptimi == false)
            {
                TournamentLiveGame = LiveTournament.MeciuriSferturi.Where(x => x.Team1.HasWon == false && x.Team2.HasWon == false).FirstOrDefault();
                if (TournamentLiveGame == null)
                {
                    LiveTournament.IsQuarter = false;
                    LiveTournament.IsSemis = true;
                    foreach (var meci in LiveTournament.MeciuriSemiFinale)
                    {
                        TournamentLiveGame = LiveTournament.MeciuriSemiFinale.Where(x => x.Team1.HasWon == false && x.Team2.HasWon == false).FirstOrDefault();
                    }
                }
            }

            if (LiveTournament.IsQuarter == false && LiveTournament.IsOptimi == false)
            {
                if (LiveTournament.IsSemis == true)
                {
                    TournamentLiveGame = LiveTournament.MeciuriSemiFinale.Where(x => x.Team1.HasWon == false && x.Team2.HasWon == false).FirstOrDefault();
                }
            }

            if (TournamentLiveGame == null && LiveTournament.IsSemis == true)
            {
                LiveTournament.IsSemis = false;
                LiveTournament.IsFinal = true;
                LiveTournament.MeciFinal.Team1.HasWon = false;
                LiveTournament.MeciFinal.Team2.HasWon = false;
            }

            if (LiveTournament.IsFinal == true)
            {
                TournamentLiveGame = LiveTournament.MeciFinal;
            }

            //if (LiveTournament.IsSemis || LiveTournament.IsQuarter || LiveTournament.IsFinal)
            //{
            //    if (TournamentLiveGame.Team1.Payout?.Any(x => x.Payout == 0) == true)
            //    {
            //        SaveInPlayGame(new InPlayGame
            //        {
            //            Game = GetGame(TournamentLiveGame.Team1.Nume),
            //            InHuntNumber = ""
            //        });
            //    }
            //    else
            //    {
            //        SaveInPlayGame(new InPlayGame
            //        {
            //            Game = GetGame(TournamentLiveGame.Team2.Nume),
            //            InHuntNumber = ""
            //        });
            //    }
            //}

            SaveCurrentTournament(LiveTournament);
            SaveCurrentTournamentLiveGame(TournamentLiveGame);
        }

        public void UpdateTournament(Tournament tournamenInfo)
        {
            SaveCurrentTournament(tournamenInfo);
            if (tournamenInfo.IsOptimi)
            {
                SaveCurrentTournamentLiveGame(tournamenInfo.MeciuriOptimi[0]);
            }
            else
            {
                SaveCurrentTournamentLiveGame(tournamenInfo.MeciuriSferturi[0]);
            }
        }

        private void SaveCurrentBetting(BettingModel bettingModel)
        {
            string file = ProjectSettings.LiveBetting;
            UnmanagedFileModifier.WriteFile(file, JsonConvert.SerializeObject(bettingModel));
        }

        private void SaveTranzactii(Tranzactii tranzactii)
        {
            string file = ProjectSettings.TranzactiiFile;
            UnmanagedFileModifier.WriteFile(file, JsonConvert.SerializeObject(tranzactii));
        }

        private void SaveCurrentBonusHunt(BonusHuntFullInfo bonusHuntFullInfo)
        {
            foreach (var bonus in bonusHuntFullInfo.Bonuses)
            {
                if (bonus.Payed > 0)
                {
                    SaveRound(bonusHuntFullInfo.HuntNumber, bonus);
                }
            }

            string file = ProjectSettings.LiveBonusHuntFile;
            UnmanagedFileModifier.WriteFile(file, JsonConvert.SerializeObject(bonusHuntFullInfo));
        }

        private void SaveRound(int huntNumber, BonusHuntGridObject bonus)
        {
            Round round = new Round()
            {
                BonusHuntId = huntNumber,
                BetSize = bonus.BetSize,
                PayAmount = bonus.Payed
            };

            var game = GetGame(bonus.GameName);


            if (game.Rounds.Any(x => x.BonusHuntId == huntNumber))
            {
                var _round = round;
            }
            else
            {
                game.Rounds.Add(round);
            }

            SaveRounds(bonus.GameName, game.Rounds);

            SetInplayGame(bonus.GameName);
        }


        /// <summary>
        /// ID with - are saved from extension
        /// </summary>
        /// <param name="round"></param>
        public void SaveRecord(Round round)
        {
            var gameName = GetInPlayGame().Game.Name;
            var game = GetGame(gameName);
            var id = 0;

            if (game.Rounds?.Count > 0)
            {
                id = game?.Rounds?.Min(x => x.BonusHuntId) ?? 0;
                if (id > 0)
                {
                    id = 0;
                }
            }
                        
            id--;

            round.BonusHuntId = id;

            game.Rounds.Add(round);

            var fileName = string.Concat(gameName.Split(Path.GetInvalidFileNameChars()));
            string file = ProjectSettings.RoundsFolder + fileName.Trim().ToLower() + ".json";
            UnmanagedFileModifier.WriteFile(file, JsonConvert.SerializeObject(game.Rounds));
        }

        private void SaveRounds(string gameName, List<Round> rounds)
        {
            var fileName = string.Concat(gameName.Split(Path.GetInvalidFileNameChars()));
            string file = ProjectSettings.RoundsFolder + fileName.Trim().ToLower() + ".json";
            UnmanagedFileModifier.WriteFile(file, JsonConvert.SerializeObject(rounds));
        }

        private void SaveInPlayGame(InPlayGame inPlayGame)
        {
            string file = ProjectSettings.InPlayGame;
            UnmanagedFileModifier.WriteFile(file, JsonConvert.SerializeObject(inPlayGame));
        }

        private void SaveCurrentTournamentLiveGame(Meci bonusHuntGridObject)
        {
            string file = ProjectSettings.TournamentLiveGameFile;
            UnmanagedFileModifier.WriteFile(file, JsonConvert.SerializeObject(bonusHuntGridObject));
        }

        private void SaveCurrentTournament(Tournament tournament)
        {
            string file = ProjectSettings.TournamentFile;
            UnmanagedFileModifier.WriteFile(file, JsonConvert.SerializeObject(tournament));
        }

        public async Task<ViewerWallet> GetViewerWallet(string viewerId)
        {
            return await SQLContextManager.GetViewerWallet(viewerId);
        }

        public async Task<bool> SetYoutubeToken(string youtubetoken)
        {
            return await SQLContextManager.SetYoutubeToken(youtubetoken);
        }

        public async Task<string> GetYoutubeToken()
        {
            return await SQLContextManager.GetYoutubeToken();
        }

        public async Task<string> GetTwitchToken()
        {
            return await SQLContextManager.GetTwitchToken();
        }

        public bool SaveStreamerSettings(StreamerSettings viewerForm)
        {
            return MemoryManager.SaveStreamerSettings(viewerForm);
        }

        public bool SaveUserSettingsForStreamerPage (List<RequestFromViewerForm> viewerForm, string viewerId)
        {
            return MemoryManager.SaveUserSettingsForStreamerPage(viewerForm, viewerId);
        }

        public List<RequestFromViewerForm> GetUserSettingsForStreamerPage(string viewerId)
        {
            return MemoryManager.GetUserSettingsForStreamerPage(viewerId);
        }

        public StreamerSettings GetStreamerSettings()
        {
            return MemoryManager.StreamerSettings();
        }

        public List<Redeem> GetAllRedeems()
        {
            return MemoryManager.GetAllRedeems();
        }

        public bool SaveStreamerTwitchSubscribers(List<TwitchSubscription> response)
        {
            return MemoryManager.SaveStreamerTwitchSubscribers(response);
        }

        public List<TwitchSubscription> GetStreamerTwitchSubscribers()
        {
            return MemoryManager.GetStreamerTwitchSubscribers();
        }

        public async Task<bool> SetTwitchToken(string twitchtoken, string twitchid)
        {
            return await SQLContextManager.SetTwitchToken(twitchtoken,twitchid);
        }

        public List<DataLayer.YTMember> GetStreamerYoutubeSubscribers()
        {
            return MemoryManager.GetStreamerYoutubeSubscribers();
        }

        public async Task<bool> IsUserValidAsync(string userID, string username)
        {
            Viewer viewer = await SQLContextManager.GetViewerModel(userID);
            if (viewer != null)
            {
                if (string.IsNullOrEmpty(username))
                    { return true; }

                if(viewer.Name != username)
                {
                    viewer.Name = username;
                    await SQLContextManager.SaveUser(viewer);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SaveStreamerYoutubeSubscribers(List<YTMember> subs)
        {
            return MemoryManager.SaveStreamerYoutubeSubscribers(subs);
        }

        public SlotsRumbleModel GetRumbleSlots()
        {
            return MemoryManager.GetRumbleSlots();
        }

        public bool AddRumbleGame(RumbleMeci rumbleMeci)
        {
            return MemoryManager.AddRumbleGame(rumbleMeci);
        }

        public bool ArchiveRumble()
        {
            return MemoryManager.NewSlotsRumble();
        }

        public bool RumbleUpdate(SlotsRumbleModel rumbleModel)
        {
            return MemoryManager.RumbleUpdate(rumbleModel);
        }

        public async Task<List<UserFullProfile>> GetAllViewers()
        {
            List<UserFullProfile> toRet = new List<UserFullProfile>();
            var usrs = MemoryManager.GetAllViewersSettings();
            foreach(var usrId in usrs.Keys)
            {
                try
                {
                    var viewer = await GetViewerAsync(usrId);
                    if (viewer != null)
                    {
                        toRet.Add(new UserFullProfile()
                        {
                            UserId = usrId,
                            Email = viewer.Email,
                            Name = viewer.Name,
                            Coins = (await GetViewerWallet(usrId)).Coins,
                            UserSettings = usrs[usrId]
                        });
                    }
                }
                catch (Exception ex)
                {
                    toRet.Add(new UserFullProfile()
                    {
                        UserId = usrId,
                        Email = ex.Message,
                        Name = ex.InnerException.Message,
                        //Coins = (await GetViewerWallet(usrId)).Coins,
                        //UserSettings = usrs[usrId]
                    });
                }
            }

            return toRet;
        }

        public bool SaveMembersMap(List<YoutubeMemberLevelMap> levelMaps)
        {
            return MemoryManager.SetMemberLevelMap(levelMaps);
        }

        public List<YoutubeMemberLevelMap> GetMemberLevelMap()
        {
            return MemoryManager.GetMemberLevelMap();
        }

        public List<Promo> GetUserPromos()
        {
            return MemoryManager.GetUserPromos();
        }

        public bool SaveUserPromos(List<Promo> promos)
        {
            return MemoryManager.SaveUserPromos(promos);
        }

        public List<PromoClicks> GetUserPromosClicks()
        {
            return MemoryManager.GetUserPromosClicks();
        }

        public bool SaveUserPromosClick(List<PromoClicks> upClick)
        {
            return MemoryManager.SaveUserPromosClick(upClick);
        }
    }
}
