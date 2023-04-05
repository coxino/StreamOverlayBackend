using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using Settings;
using static System.Net.WebRequestMethods;

namespace MemoryManipulator
{
    public class CoxinoLockObjects
    {
        public static object LoyaltyFile = new object();
        public static object BettingFile = new object(); 
        public static object CooldownFile = new object(); 
        public static object ViewerBet = new object();
        public static object HotWords = new object();
        public static object BroadcastFile = new object();
        public static object RedeemsFile = new object();

        public static object LockPacaniada = new object();

        public static object SubscriberObject = new object();

        public static object RumbleObject = new object();
        public static object MemberMap = new object();
    }
    public class GinionistiiLock
    {
        public static object LockLigaFile = new object();
        //public static object LockLigaFile;
    }

    public class MemoryManager
    {
        private string userId;

        public MemoryManager(string userId)
        {
            this.userId = userId;
        }

        public List<LigaUser> GetLiga()
        {
            string file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LigaFile;
            lock (GinionistiiLock.LockLigaFile)
            {
                return FileReader.ReadFile<List<LigaUser>>(file);
            }
        }

        public ClasamentPacaniada GetPacaniada()
        {
            string file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.Pacaniada;
            lock (CoxinoLockObjects.LockPacaniada)
            {
                return FileReader.ReadFile<ClasamentPacaniada>(file);
            }
        }

        public bool SetPacaniada(ClasamentPacaniada clasament)
        {
            string file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.Pacaniada;
            lock (CoxinoLockObjects.LockPacaniada)
            {
                return FileWriter.SaveData(file, clasament, CoxinoLockObjects.LockPacaniada);
            }
        }

        public bool SaveShop(List<ShopItem> shopItems)
        {
            string file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.Shop;
            lock (CoxinoLockObjects.LockPacaniada)
            {
                return FileWriter.SaveData(file, shopItems, CoxinoLockObjects.LockPacaniada);
            }
        }

        public bool AddLiga(LigaUser ligaUser)
        {
            string file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LigaFile;

            var liga = GetLiga() ?? new List<LigaUser>();
            if (liga.Any(x => x.Nume == ligaUser.Nume))
            {
                var user = liga.Where(x => x.Nume == ligaUser.Nume).FirstOrDefault();

                if(user.Speciale.Count != ligaUser.Rank.Sanse)
                {
                    ligaUser.Speciale = ligaUser.Speciale.OrderByDescending(x => x.Payed / x.BuyIn).Take(ligaUser.Rank.Sanse).ToList();
                    if(user.Speciale.Count != ligaUser.Rank.Sanse)
                    {
                        var toadd = ligaUser.Rank.Sanse - user.Speciale.Count;
                        for (int i = 0; i < toadd; i++)
                        {
                            ligaUser.Speciale.Add(new Speciala() { BuyIn = 0.8, Payed = 0, Nume = "" });
                        }
                    }
                }

                liga.Remove(user);
                liga.Add(ligaUser);
            }
            else
            {
                ligaUser.Speciale = new List<Speciala>();

                for (int i = 0; i < ligaUser.Rank.Sanse; i++)
                {
                    ligaUser.Speciale.Add(new Speciala() {BuyIn=0.8,Payed=0,Nume="" });
                }

                liga.Add(ligaUser);
            }

            liga = liga.OrderByDescending(x => x.Speciale.Max(x => x.Payed / x.BuyIn)).ToList();
            
            return FileWriter.SaveData(file, liga, GinionistiiLock.LockLigaFile);
        }

        public void BroadcastMessage(string userName, string message)
        {
            string file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.Broadcasts;
            FileWriter.SaveData(file, new { userName = userName, message = message }, CoxinoLockObjects.BroadcastFile);
        }

        public object GetBroadcasts()
        {
            string file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.Broadcasts;
            return FileReader.ReadFile<List<object>>(file) ?? new object { };
        }

        public bool StergeUserDinLiga(LigaUser user)
        {
            var liga = GetLiga();
            var q = liga.FirstOrDefault(x => x.Nume == user.Nume);
            liga.Remove(q);


            liga = liga.OrderByDescending(x => x.Speciale.Max(x => x.Payed / x.BuyIn)).ToList();
            string file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LigaFile;
            return FileWriter.SaveData(file, liga, GinionistiiLock.LockLigaFile);
        }

        public BettingModel GetLiveBeting()
        {            
            string file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LiveBetting;
            lock (CoxinoLockObjects.BettingFile)
            {
                return FileReader.ReadFile<BettingModel>(file) ?? new BettingModel();
            }
        }

        public void PutUserOnCooldown(List<UserCooldown> cd, string cmd)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.CooldownFolder + cmd + ProjectSettings.CooldownFile;
            FileWriter.SaveData(file, cd, CoxinoLockObjects.CooldownFile);            
        }

        public void SaveLiveBetting(BettingModel bettingModel)
        {
            string file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LiveBetting;
            FileWriter.SaveData(file, bettingModel, CoxinoLockObjects.BettingFile);           
        }

        public string SaveUserBets(string file, string viewerID, int amount, string key)
        {
            file =  ProjectSettings.DatabaseFolder + userId + ProjectSettings.LiveBettingUserOptions + key + ".json";

            UserBet userBet = new UserBet()
            {
                UserID = viewerID,
                Bet = amount
            };

            var userBets = FileReader.ReadFile<List<UserBet>>(file) ?? new List<UserBet>();
            userBets.Add(userBet);
            FileWriter.AppendData(file, userBets, CoxinoLockObjects.ViewerBet);
            return "vote registered!";
        }

        public string SaveHotWords(string file, object v)
        {
            file = ProjectSettings.DatabaseFolder + userId + file;
            return FileWriter.SaveData(file, v, CoxinoLockObjects.HotWords) ? "" : "";
        }

        public LoyalityRanking GetUserLoyals()
        {
            var file = @"C:\API\database\coxino\Loyality\usersPoints.json";
            return FileReader.ReadFile<LoyalityRanking>(file) ?? new LoyalityRanking();
        }

        public List<UserBet> GetUserBets(string bettingOption)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LiveBettingUserOptions + bettingOption + ".json";
            return FileReader.ReadFile<List<UserBet>>(file);
        }

        public List<HotWord> GetHotWords()
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.HotWords;
            return FileReader.ReadFile<List<HotWord>>(file) ?? new List<HotWord>();
        }

        public int ReadCoins(string userID)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LoyaltyRanking + userID + ".json";
            return FileReader.ReadFile<int>(file);
        }

        public bool SaveStreamerSettings(StreamerSettings viewerForm)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LoyaltyRanking + "RequestFromViewerForm.json";
            return FileWriter.SaveData(file, viewerForm, CoxinoLockObjects.LoyaltyFile);
        }

        public StreamerSettings StreamerSettings()
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LoyaltyRanking + "RequestFromViewerForm.json";
            return  FileReader.ReadFile<StreamerSettings>(file);
        }

        public bool SaveUserSettingsForStreamerPage(List<RequestFromViewerForm> viewerForm,string viewerId)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LoyaltyRanking + ProjectSettings.LoyaltyUserSettings + viewerId + ".json";
            return FileWriter.SaveData(file, viewerForm, CoxinoLockObjects.LoyaltyFile);
        }

        public List<RequestFromViewerForm> GetUserSettingsForStreamerPage(string viewerId)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LoyaltyRanking + ProjectSettings.LoyaltyUserSettings + viewerId + ".json";
            return FileReader.ReadFile<List<RequestFromViewerForm>>(file);
        }

        public Dictionary<string,List<RequestFromViewerForm>> GetAllViewersSettings()
        {
            Dictionary<string, List<RequestFromViewerForm>> users = new Dictionary<string, List<RequestFromViewerForm>>();
            var dir = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LoyaltyRanking + ProjectSettings.LoyaltyUserSettings;
            
            var dirFiles = Directory.GetFiles(dir, "*.json", SearchOption.AllDirectories);

            foreach(var file in dirFiles)
            {
                try
                {
                    var fn = Path.GetFileNameWithoutExtension(file);
                    if (!string.IsNullOrWhiteSpace(fn))
                        users.Add(fn, GetUserSettingsForStreamerPage(fn));
                }
                catch(Exception ex)
                {
                    var a = ex.Message;
                }
            }

            return users;
        }

        public List<Redeem> GetAllRedeems()
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.RedeemsFile;
            return FileReader.ReadFile<List<Redeem>>(file);
        }

        public void SaveRedeem(Redeem redeem)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.RedeemsFile;
            var redeems = GetAllRedeems();
            if(redeems == null)
            {
                redeems = new List<Redeem>();
            }
            redeems.Add(redeem);
            FileWriter.SaveData(file, redeems, CoxinoLockObjects.RedeemsFile);            
        }

        public bool SaveStreamerTwitchSubscribers(List<TwitchSubscription> response)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LoyaltyRanking + "TwitchSubscribers.json";
           return FileWriter.SaveData(file, response, CoxinoLockObjects.SubscriberObject);
        }

        public List<TwitchSubscription> GetStreamerTwitchSubscribers()
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LoyaltyRanking + "TwitchSubscribers.json";            
            return FileReader.ReadFile<List<TwitchSubscription>>(file);
        }

        public void DeleteAllColldown(string v)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.CooldownFolder + v + ProjectSettings.CooldownFile;
            System.IO.File.Delete(file);
        }

        public bool SaveStreamerYoutubeSubscribers(List<YTMember> response)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LoyaltyRanking + "YoutubeSubscribers.json";
            return FileWriter.SaveData(file, response, CoxinoLockObjects.SubscriberObject);
        }

        public List<YTMember> GetStreamerYoutubeSubscribers()
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LoyaltyRanking + "YoutubeSubscribers.json";
            return FileReader.ReadFile<List<YTMember>>(file);
        }

        public SlotsRumbleModel GetRumbleSlots()
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.SlotsRumble;
            return FileReader.ReadFile<SlotsRumbleModel>(file);
        }

        public bool AddRumbleGame(RumbleMeci rumbleMeci)
        {
            var rma = GetRumbleSlots();
            rma.GameHistory.Add(rumbleMeci);

            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.SlotsRumble;
            return FileWriter.SaveData(file, rma, CoxinoLockObjects.RumbleObject);
        }

        private bool ArchiveRumble(SlotsRumbleModel rma)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.SlotsRumbleArchive + rma.RumbleId + ".json";
            return FileWriter.SaveData(file, rma, CoxinoLockObjects.RumbleObject);
        }

        public bool NewSlotsRumble()
        {
            var rma = GetRumbleSlots();
            if (ArchiveRumble(rma))
            {
                var newRma = new SlotsRumbleModel() { RumbleId = rma.RumbleId + 1, GameHistory = new List<RumbleMeci>() { new RumbleMeci() } };
                var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.SlotsRumble;
                return FileWriter.SaveData(file, newRma, CoxinoLockObjects.RumbleObject);
            }
            return false;
        }

        public bool RumbleUpdate(SlotsRumbleModel rumbleModel)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.SlotsRumble;
            return FileWriter.SaveData(file, rumbleModel, CoxinoLockObjects.RumbleObject);
        }

        public MemberLevels GetMemberLevels(string slevel)
        {
            try
            {
                var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.MemberMaps;
                var q = FileReader.ReadFile<List<YoutubeMemberLevelMap>>(file);

                return q.FirstOrDefault(x => x.MemberLevelString == slevel).MemberLevelEnum;
            }
            catch
            {
                return MemberLevels.Level1;
            }
        }

        public bool SetMemberLevelMap(List<YoutubeMemberLevelMap> levelMaps)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.MemberMaps;
            return FileWriter.SaveData(file, levelMaps, CoxinoLockObjects.MemberMap);
        }

        public List<YoutubeMemberLevelMap> GetMemberLevelMap()
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.MemberMaps;
            return FileReader.ReadFile<List<YoutubeMemberLevelMap>>(file);
        }

        public List<Promo> GetUserPromos()
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.UserPromos;
            return FileReader.ReadFile<List<Promo>>(file);
        }

        public bool SaveUserPromos(List<Promo> promos)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.UserPromos;
            return FileWriter.SaveData(file, promos, CoxinoLockObjects.MemberMap);
        }

        public List<PromoClicks> GetUserPromosClicks()
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.UserPromosClicks;
            return FileReader.ReadFile<List<PromoClicks>>(file) ?? new List<PromoClicks>();
        }

        public bool SaveUserPromosClick(object promos)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.UserPromosClicks;
            return FileWriter.SaveData(file, promos, CoxinoLockObjects.MemberMap);
        }
    }
}