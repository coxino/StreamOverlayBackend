﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using Settings;

namespace MemoryManipulator
{
    public class CoxinoLockObjects
    {
        public static object LoyaltyFile = new object();
        public static object BettingFile = new object(); 
        public static object CooldownFile = new object(); 
        public static object ViewerBet = new object();
        public static object HotWords = new object();
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

        public bool StergeUserDinLiga(LigaUser user)
        {
            var liga = GetLiga();
            var q = liga.FirstOrDefault(x => x.Nume == user.Nume);
            liga.Remove(q);


            liga = liga.OrderByDescending(x => x.Speciale.Max(x => x.Payed / x.BuyIn)).ToList();
            string file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LigaFile;
            return FileWriter.SaveData(file, liga, GinionistiiLock.LockLigaFile);
        }

        public List<BettingModel> GetLiveBeting()
        {            
            string file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LiveBetting;
            lock (CoxinoLockObjects.BettingFile)
            {
                return FileReader.ReadFile<List<BettingModel>>(file) ?? new List<BettingModel>();
            }
        }

        public void PutUserOnCooldown(List<UserCooldown> cd, string cmd)
        {
            var file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.CooldownFolder + cmd + ProjectSettings.CooldownFile;
            FileWriter.SaveData(file, cd, CoxinoLockObjects.CooldownFile);            
        }

        public void SaveLiveBetting(List<BettingModel> bettingModel)
        {
            string file = ProjectSettings.DatabaseFolder + userId + ProjectSettings.LiveBetting;
            FileWriter.SaveData(file, bettingModel, CoxinoLockObjects.BettingFile);           
        }

        public string SaveUserBets(string file, string viewerID, string user, string amount, string key)
        {
            file =  ProjectSettings.DatabaseFolder + userId + ProjectSettings.LiveBettingUserOptions + key + ".json";

            UserBet userBet = new UserBet()
            {
                UserID = viewerID,
                Name = user,
                Bet = int.Parse(amount)
            };

            var userBets = FileReader.ReadFile<List<UserBet>>(file) ?? new List<UserBet>();
            userBets.Add(userBet);
            FileWriter.AppendData(file, userBets, CoxinoLockObjects.ViewerBet);
            return string.Format("@{0} votul a fost inregistrat!", userBet.Name);
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
    }
}