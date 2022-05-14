using DatabaseContext;
using DataLayer;
using JWTManager;
using LocalDatabseManager;
using MemoryManipulator;
using Newtonsoft.Json;
using Settings;
using SQLContextManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LocalDatabaseManager
{
    public class Database
    {
        DatabaseReadWrite UnmanagedFileModifier;
        MemoryManager MemoryManager;
        public SQLContex SQLContextManager;       
        public ValidationModel ValidationResponse { get; set; }

        public Database(string username = "")
        {
            UnmanagedFileModifier = new DatabaseReadWrite(username);
            MemoryManager = new MemoryManager(username);
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

        public List<ShopItem> GetShop()
        {
            return UnmanagedFileModifier.ReadFile<List<ShopItem>>(ProjectSettings.Shop);
        }

        public void UpdateBetting(BettingModel bettingModel)
        {
            MemoryManager.SaveLiveBetting(bettingModel);
        }

        public async Task<string> BuyGiveawayTiket(BuyTiketModel buyTiketModel)
        {
            var viewer = await SQLContextManager.GetUserLoyalty(buyTiketModel.userID);
            GivewayModel giveway = await SQLContextManager.GetGivewayModel(buyTiketModel.givewayID);

            var userTikets = await SQLContextManager.GetViewerGivewayTokens(viewer.Id, giveway.Id);

            if(userTikets >= giveway.MaxTikets)
            {
                return $"Nu poti cumpara mai mult de {giveway.MaxTikets} bilete pentru acest giveaway.";
            }


            if (viewer.MemberLevel > MemberLevels.Coxumator)
            {
                if (viewer.Inventory < giveway.Price * 0.8)
                {
                    return "Nu ai destul cox! Stai pe live si aduna cox!";
                }

                if (await SQLContextManager.CreateGivewayTiket(viewer, giveway))
                {
                    if (await SQLContextManager.AddPointToViewerAsync(viewer, (-1 * (int)(giveway.Price * 0.8)),false))
                    {
                        int cnt = await SQLContextManager.GetViewerGivewayTokens(viewer.Id, giveway.Id);
                        int cntTotal = await SQLContextManager.GetGivewayTiketCount(giveway.Id);
                        var percent = (double)cnt / cntTotal * 100;
                        return $"Felicitari ai fost inscris in giveaway! Acum ai {cnt} tikete din {cntTotal} si ai sanse de {percent:00.00}% sa castigi giveaway-ul. Mai multe bilete mai multe sanse de castig!";
                    }
                }
            }
            else
            {
                if (viewer.Inventory < giveway.Price)
                {
                    return "Nu ai destule cox! Stai pe live si aduna cox!";
                }

                if (await SQLContextManager.CreateGivewayTiket(viewer, giveway))
                {
                    if (await SQLContextManager.AddPointToViewerAsync(viewer, (-1 * giveway.Price),false))
                    {
                        int cnt = await SQLContextManager.GetViewerGivewayTokens(viewer.Id, giveway.Id);
                        int cntTotal = await SQLContextManager.GetGivewayTiketCount(giveway.Id);
                        var percent = (double)cnt / cntTotal * 100;
                        return $"Felicitari ai fost inscris in giveaway! Acum ai {cnt} tikete din {cntTotal} si ai sanse de {percent:00.00}% sa castigi giveaway-ul. Mai multe bilete mai multe sanse de castig!";
                    }
                }
            }
            return "Ceva a mers prost... Incearca mai tarziu!";
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
            return await SQLContextManager.SetUserActive(viewerID,viewerName);
        }

        public async Task<Viewer> GetViewerAsync(string viewerID)
        {
            return await SQLContextManager.GetUserLoyalty(viewerID);
        }

        public async Task<CoxiUser> GetCoxiCoinsAsync(string userID, string email, string ipadress, string numeSuperbet,string nume)
        {
            var user = await SQLContextManager.GetUserLoyalty(userID);

            if (user == null)
            {
                user = new Viewer()
                {
                    Name = nume,
                    Id = userID,
                    Inventory = 25,
                    Email = email,
                    Ipadress = ipadress,
                    CreationTime = DateTime.Now,
                    SuperbetName = numeSuperbet,
                    LastActive = DateTime.Now,
                    MemberLevel = MemberLevels.Viewer
                };
                if (await SaveLoyaltyAsync(user))
                {
                    return new CoxiUser() { CoxiCoins = user.Inventory,NumeSuperbet = user.SuperbetName };
                }
            }
            else
            {
                bool save = false;
                if (string.IsNullOrWhiteSpace(user.Email) && string.IsNullOrWhiteSpace(email) == false)
                {
                    user.Email = email;
                    save = true;
                }
                if (string.IsNullOrWhiteSpace(user.Ipadress) && string.IsNullOrWhiteSpace(ipadress) == false)
                {
                    user.Ipadress = ipadress;
                    save = true;
                }

                if (string.IsNullOrWhiteSpace(numeSuperbet) == false)
                {
                    if (user.SuperbetName != numeSuperbet)
                    {
                        user.SuperbetName = numeSuperbet;
                        save = true;
                    }
                }

                if(save == true)
                {
                   await SQLContextManager.SaveUser(user);
                }

            }

            return new CoxiUser() { CoxiCoins = user.Inventory, NumeSuperbet = user.SuperbetName };
        }

        public async Task<string> WinPointsAsync(Viewer utilizator, int ammount)
        {
            return await SQLContextManager.AddPointToViewerAsync(utilizator, ammount) ? $"Ai primit {ammount} de cox!" : $"Din pacate nu a reusit transferul a {ammount}";
        }

        public List<LigaUser> GetLiga()
        {
            return MemoryManager.GetLiga();
        }

        public bool AddLiga(LigaUser ligaUser)
        {
            return MemoryManager.AddLiga(ligaUser);
        }

        public async Task<string> RedeemItem(Viewer user, ShopItem item, string additionaldata = "")
        {
            if ((int)user.MemberLevel > 2)
            {
                if (user.Inventory < item.Pret * 0.8)
                {
                    return "Nu ai suficiente cox!";
                }

                user.Inventory -= (int)(item.Pret * 0.8);
            }
            else
            {
                if (user.Inventory < item.Pret)
                {
                    return "Nu ai suficiente cox!";
                }

                user.Inventory -= item.Pret;
            }

            string redeemer = user.Id + "-" + user.Name + "-" + additionaldata + "\r\n";
            string file = string.Format(ProjectSettings.RedeemsFile, item.Nume);
            UnmanagedFileModifier.AppendFile(file, redeemer);

            var shop = UnmanagedFileModifier.ReadFile<List<ShopItem>>(ProjectSettings.Shop);
            shop.FirstOrDefault(x => x.ItemID == item.ItemID).Stoc -= 1;
            UnmanagedFileModifier.WriteFile(ProjectSettings.Shop, JsonConvert.SerializeObject(shop));

            return await SQLContextManager.SaveUser(user) ? $"Ai cumparat cu success {item.Nume}!" : $"Produsul nu a putut fi cumparat!";
        }

        public async Task<List<Viewer>> GetViewerByNameAsync(string userName)
        {
            return await SQLContextManager.GetViewerByNameAsync(userName);
        }

        public async Task<string> AddPointsToOneUser(string userID, int ammount,bool doubleUp = true)
        {
            return await SQLContextManager.AddPointToViewerAsync(userID, ammount, doubleUp) ? $"Ai primit {ammount} de cox!" : $"Din pacate nu a reusit transferul a {ammount}";
        }

        public async Task<int> AddPointsAllAsync(int ammount)
        {
           return await SQLContextManager.AddPointToAllViewersAndCountAsync(ammount);
        }

        public async Task<bool> SetMemberLevel(string id, int ammount)
        {
            return await SQLContextManager.SetUserLevel(id, ammount);
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

        public List<HotWord> GetHotWords()
        {
            return MemoryManager.GetHotWords();
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

        public string AddHotWord(string bettingOption)
        {
            bettingOption = Regex.Unescape(bettingOption);            
            var hotWords = GetHotWords();
            var hw = hotWords?.Where(x => x.Word.ToLower() == bettingOption.ToLower())?.FirstOrDefault();
            if (hw != null)
            {
                hw.Degree++;
            }
            else
            {
                hotWords.Add(new HotWord() { Word = bettingOption, Degree = 1 });
            }
            hotWords = hotWords.OrderByDescending(x => x.Degree).ToList();

            return SaveHotWords(hotWords);
        }

        public void CreateBettingFromBonusHunt(int maxBet)
        {
            BettingModel BettingModel = new BettingModel(); 
            BettingModel.MaxBet = maxBet;
            BettingModel.VoteTitle = "Cat plateste Bonus Hunt-ul?";
            var bh = UnmanagedFileModifier.ReadFile<BonusHuntFullInfo>(ProjectSettings.LiveBonusHuntFile);
            var bhval = bh.HuntValue;
            var step = bhval / 10;


            string opt1 = "Under " + step * 7;
            string middleOpt = "Intre {0} si {1}";
            string lastOpt = "Over " + step * 13;

            BettingModel.Options.Add(new BettingOptionModel(0, maxBet)
            {
                Nume = opt1
            });

            for (int i = 1; i < 7; i++)
            {
                BettingModel.Options.Add(new BettingOptionModel(i, maxBet)
                {
                    Nume = string.Format(middleOpt, step * (6 + i), step * (7 + i))
                });
            }

            BettingModel.Options.Add(new BettingOptionModel(7, maxBet)
            {
                Nume = lastOpt
            });

            SetLiveBetting(BettingModel);
        }

        public void ResetHotWors()
        {
            string file = ProjectSettings.HotWords;
            UnmanagedFileModifier.WriteFile(file, JsonConvert.SerializeObject(new List<HotWord>()));
        }

        public void TestFile(string v, string toAppend)
        {
            UnmanagedFileModifier.WriteFile(v, toAppend);
        }

        private string SaveHotWords(List<HotWord> hotWords)
        {
            string file = ProjectSettings.HotWords;
            return MemoryManager.SaveHotWords(file,hotWords);
        }

        public void AddSingleGameToBH(InPlayGame gameName, int betSize)
        {
            var bonushunt = GetLiveBonusHunt();
            if (bonushunt != null)
            {
                bonushunt.Bonuses.Add(new BonusHuntGridObject() { BetSize = betSize, GameName = gameName.Game.Name, Payed = 0, IsCurrent = false, ProviderName = gameName.Game.Provider });
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
                Viewer viewer = await SQLContextManager.GetUserLoyalty(userID);
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

        public void AddUserOnCooldown(string userID, string cmd, double cooldown = 5)
        {
            var cd = UnmanagedFileModifier.ReadFile<List<UserCooldown>>(ProjectSettings.CooldownFolder + cmd + ProjectSettings.CooldownFile) ?? new List<UserCooldown>();
            if (cd.Any(x => x.userID == userID))
            {
                cd.RemoveAll(x => x.userID == userID);
            }

            cd.Add(new UserCooldown() { userID = userID, Expires = DateTime.Now.AddMinutes(cooldown) });

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
                SaveInPlayGame(new InPlayGame() { Game = inDbGame, BonusHuntPreInfo = getInfoFromBH(bonushunt), InHuntNumber = bonushunt.Bonuses.Where(x => x.Payed > 0).Count() + "/" + bonushunt.Bonuses.Count() });
            else
                SaveInPlayGame(new InPlayGame() { Game = new Game() { Name = inPlayGame.Game.Name }, BonusHuntPreInfo = getInfoFromBH(bonushunt), InHuntNumber = bonushunt.Bonuses.Where(x => x.Payed > 0).Count() + "/" + bonushunt.Bonuses.Count() });
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
            Viewer jackpotWinner = null;

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
            if (jackpotWinner != null)
            {
                jackpotWinner.Inventory += jackpotAmmount;

            }
            else
            {
                jackpotWinner = new Viewer()
                {
                    Name = "NOBODY",
                };
            }

            UnmanagedFileModifier.RemoveBettingFiles();
            
                return "Option -" + bettingOption + "- won the pool," +
                "all betters recieved " + ammountToRecieve + " for each point they " +
                "bet! In a total of " + totalSpent + " spread to " + userBets.Count + " winners. " +
                "@" + jackpotWinner.Name + " Won the jackpot in value of" +
                " " + jackpotAmmount + " points.";
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

        public async Task<string> IncrementBettingOptionAsync(string userID, string key, string userName, string amount)
        {
            try
            {
                if (IsUserOnCooldown(userID, "bet"))
                {
                    return string.Format("@{0} ai votat deja!", userName);
                }
                var betting = GetLiveBetting();
                var lp = await SQLContextManager.GetLoyalityAsync();

                if(lp.Any(x=>x.Id == userID) == false)
                {
                     return await CreateUserFromScrapAsync(userID, userName) + ". Votul nu a fost inregistrat!!!";
                }

                var userLoyals = lp.Where(x => x.Id == userID).FirstOrDefault();
               
                if (string.IsNullOrWhiteSpace(userLoyals.Name))
                {
                    userLoyals.Name = userName;
                }
                if (int.TryParse(amount, out int amm))
                {
                    if (amm > betting.MaxBet)
                    {
                        return string.Format("@{0} max bet este {1}!", userName, betting.MaxBet);
                    }

                    if (userLoyals.Inventory < amm)
                    {
                        return string.Format("@{0} ai {1} de cox nu poti paria {2} de cox!", userName, userLoyals.Inventory, amm);
                    }

                }

                var opt = betting.Options.Where(x => x.Key == key)?.FirstOrDefault();
                
                if (opt != null)
                {
                    opt.Voturi++;
                    opt.TotalPariat += amm;
                }
                else
                {
                    return string.Format("@{0} ai votat o optiune invalida! Incearca una din urmatoarele optiuni" + betting.Options.Select(x => x.Key).ToString(), userName);
                }

                //Update Progesses
                var totalbets = betting.Options.Sum(x => x.Voturi);
                foreach (var op in betting.Options)
                {
                    op.Progress = (int)Math.Round(((double)op.Voturi / totalbets * 100));
                }

                if (betting.Options.Sum(x => x.Progress) < 100)
                {
                    var maxno = betting.Options.Max(x => x.Progress);

                    var max = betting.Options.Where(x => x.Progress == maxno).FirstOrDefault();
                    max.Progress = maxno + 100 - betting.Options.Sum(x => x.Progress);
                }

                if (betting.Options.Sum(x => x.Progress) > 100)
                {
                    var maxno = betting.Options.Max(x => x.Progress);

                    var max = betting.Options.Where(x => x.Progress == maxno).FirstOrDefault();
                    max.Progress = maxno - (betting.Options.Sum(x => x.Progress) - 100);
                }

                userLoyals.Inventory -= amm;
                await SaveLoyaltyAsync(userLoyals);
                SetLiveBetting(betting);
                AddUserOnCooldown(userID, "bet", 25);
                string response = SaveUsersBets(userID: userID, key: key, user: userName, amount: amount);

                return response;
            }
            catch
            {
                return string.Format("@{0} ceva nu a mers bine incearca din nou!", userName);
            }
        }

        private async Task<string> CreateUserFromScrapAsync(string userID, string userName, int ammount = 10)
        {
            Viewer viewer = new Viewer()
            {
                CreationTime = DateTime.Now,
                Id = userID,
                Inventory = ammount,
                Name = userName,
                LastActive = DateTime.Now
            };

            if (await SQLContextManager.CreateUser(viewer))
            {
                return string.Format("@{0} te-ai inscris in sistemul de loyalitate si ai primit 10 cox, nu uita sa te inscrii si pe https://coxino.ro/shop , acum poti sa votezi!", userName);
            }
            else
            {
                Viewer viewer2 = new Viewer()
                {
                    CreationTime = DateTime.Now,
                    Id = userID,
                    Inventory = ammount,
                    Name = userName.ToAscii(),
                    LastActive = DateTime.Now
                };
                if (await SQLContextManager.CreateUser(viewer2))
                {
                    return string.Format("@{0} te-ai inscris in sistemul de loyalitate si ai primit 10 cox dar din cauza caracterelor speciale din numele tau, este posibil ca unele functii sa nu mearga corespunzator. numele tau in sistem va fi : {1}", userName, viewer2.Name);
                }
                else
                {
                    return string.Format("@{0} nu am putut sa te inscriu in sistemul de loyalitate din cauza caracterelor speciale din numele tau, schimba numele sau incearca direct pe site! https://coxino.ro/shop", userName);
                }
            }
            }

        private string SaveUsersBets(string userID, string key, string user, string amount)
        {
            string file = ProjectSettings.LiveBettingUserOptions + key + ".json";
            return MemoryManager.SaveUserBets(file, viewerID: userID, user, amount, key);
        }

        public void CreateBettingFromTournament(int maxBet)
        {
            BettingModel BettingModel = new BettingModel();
            BettingModel.MaxBet = maxBet;
            BettingModel.VoteTitle = "Voteaza-ti preferatul!";
            List<string> allNames = GetLiveTournament().MeciuriSferturi.Select(x => x.Team1.Nume).ToList();
            allNames.AddRange(GetLiveTournament().MeciuriSferturi.Select(x => x.Team2.Nume).ToList());
            for (int i = 0; i < 8; i++)
            {
                BettingModel.Options.Add(new BettingOptionModel(i, maxBet)
                {
                    Nume = allNames[i],
                });
            }

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
            var cnt = 0;
            if (!bonusHunt.Bonuses.Any(x => x.BetSize <= 0))
            {
                bonusHunt.Bonuses = bonusHunt.Bonuses.OrderBy(x => x.BetSize).ToList();
            }

            foreach (var game in bonusHunt.Bonuses)
            {
                game.IsCurrent = false;
            }
            BonusHuntGridObject inplay = new BonusHuntGridObject()
            {
                GameName = "Hunting",
                ProviderName = "",
            };

            if (bonusHunt.IsHunting == false)
            {
                inplay = bonusHunt.Bonuses.Where(x => x.Payed <= 0).FirstOrDefault();
                cnt = bonusHunt.Bonuses.IndexOf(inplay) + 1;
            }

            BonusHuntPreInfo bonusHuntPreInfo = getInfoFromBH(bonusHunt);
            if (inplay != null)
            {
                inplay.IsCurrent = true;
                SaveInPlayGame(inplay, bonusHuntPreInfo, cnt, bonusHunt.Bonuses.Count);
            }

            SaveCurrentBonusHunt(bonusHunt);
        }

        private BonusHuntPreInfo getInfoFromBH(BonusHuntFullInfo bonusHunt)
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
                InHuntNumber = cnt + "/" + total,
                BonusHuntPreInfo = bonusHuntPreInfo
            };

            SaveInPlayGame(inPlay);
        }

        private Game GetGame(string gameName)
        {
            var game = UnmanagedFileModifier.GetGame(gameName) ?? new Game() { Name = gameName };

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
            if(TournamentLiveGame == null)
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

                if(LiveTournament.IsOptimi == true)
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
                if(TournamentLiveGame == null)
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
    }
}
