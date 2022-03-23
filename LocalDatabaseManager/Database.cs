using DataLayer;
using Newtonsoft.Json;
using Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace LocalDatabaseManager
{
    public class Database
    {
        DatabaseReadWrite database;

        public Database(string userId)
        {
            this.database = new DatabaseReadWrite(userId);
        }

        public InPlayGame GetInPlayGame()
        {
            return database.ReadFile<InPlayGame>(ProjectSettings.InPlayGame);
        }

        public string GetCustomTheme()
        {
            return database.ReadString(ProjectSettings.CustomThemeFile);
        }

        public Tranzactii GetTranzactii()
        {            
            return database.ReadFile<Tranzactii>(ProjectSettings.TranzactiiFile);
        }

        public void SetCustomTheme(string customTheme)
        {
            string file = ProjectSettings.CustomThemeFile;
            database.WriteFile(file, customTheme);
        }

        public BonusHuntFullInfo GetLiveBonusHunt()
        {
            return database.ReadFile<BonusHuntFullInfo>(ProjectSettings.LiveBonusHuntFile);
        }

        public bool SetInplayGame(InPlayGame inPlayGame)
        {
            var inDbGame = GetGame(inPlayGame.Game.Name);
            if (inDbGame != null)
            {
                var dbchange = false;
                if (!string.IsNullOrWhiteSpace(inPlayGame.Game.Potential))
                {
                    if (inDbGame.Potential != inPlayGame.Game.Potential)
                    {
                        inDbGame.Potential = inPlayGame.Game.Potential;
                        dbchange = true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(inPlayGame.Game.Volatility))
                {
                    if (inDbGame.Volatility != inPlayGame.Game.Volatility)
                    {
                        inDbGame.Volatility = inPlayGame.Game.Volatility;
                        dbchange = true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(inPlayGame.Game.Provider))
                {
                    if (inDbGame.Provider != inPlayGame.Game.Provider)
                    {
                        inDbGame.Provider = inPlayGame.Game.Provider;
                        dbchange = true;
                    }
                }

                if (dbchange)
                {
                    UpdateGame(inDbGame);
                }
            }
            else
            {
                //to be added on a core
                //if (database.ApplicationUser.Role == UserRoles.Administrator)
                //{
                //    UpdateGame(new Game()
                //    {
                //        Name = inPlayGame.Game.Name,
                //        Image = inPlayGame.Game.Image,
                //        Potential = inPlayGame.Game.Potential,
                //        Provider = inPlayGame.Game.Provider,
                //        Rounds = inPlayGame.Game.Rounds,
                //        Volatility = inPlayGame.Game.Volatility
                //    });
                //}
            }

            SaveInPlayGame(inPlayGame);
            return true;
        }

        private void UpdateGame(Game inDbGame)
        {
            throw new NotImplementedException();
        }

        public Meci GetLiveTournamentLiveFight()
        {
            return database.ReadFile<Meci>(ProjectSettings.TournamentLiveGameFile);
        }

        public void SetTranzactii(Tranzactii tranzactii)
        {
            SaveTranzactii(tranzactii);
        }

        public Tournament GetLiveTournament()
        {
           return database.ReadFile<Tournament>(ProjectSettings.TournamentFile);
        }

        public void SetCustomTheme()
        {
            throw new NotImplementedException();
        }

        public void UpdateBonusHunt(BonusHuntFullInfo bonusHunt)
        {
            foreach (var game in bonusHunt.Bonuses)
            {
                game.IsCurrent = false;
            }

            var inplay = bonusHunt.Bonuses.Where(x => x.Payed <= 0).FirstOrDefault();
                     
            if (inplay != null)
            {
                inplay.IsCurrent = true;
                SaveInPlayGame(inplay);
            }
           SaveCurrentBonusHunt(bonusHunt);
        }

        private void SaveInPlayGame(BonusHuntGridObject inplay)
        {
            InPlayGame inPlay = new InPlayGame()
            {
                Game = GetGame(inplay.GameName),
                InHuntNumber = GetLiveBonusHunt().HuntNumber,

            };

            SaveInPlayGame(inPlay);
        }

        private Game GetGame(string gameName)
        {   
            var game = database.GetGame(gameName) ?? new Game() { Name = gameName };           

            return game;
        }
    

        public void UpdateTournamentLiveFight(string json)
        {
            string file = ProjectSettings.TournamentLiveGameFile;
            database.WriteFile(file, json);
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
            database.WriteFile(file, JsonConvert.SerializeObject(t));
        }

        public void CloseCurrentFight()
        {
            CloseCurrentTournamentLiveFight();
        }

        //public

        private void CloseCurrentTournamentLiveFight()
        {
            var LiveTournament = GetLiveTournament();
            var aux = GetLiveTournamentLiveFight();

            var TournamentLiveGame = LiveTournament.MeciuriSferturi.Where(x => x.Team1.Nume == aux.Team1.Nume && x.Team2.Nume == aux.Team2.Nume).FirstOrDefault();
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

                if (LiveTournament.IsQuarter == true)
                {
                    var nextBattle = LiveTournament.MeciuriSemiFinale
                         .Where(x => string.IsNullOrWhiteSpace(x.Team1.Nume) || string.IsNullOrWhiteSpace(x.Team2.Nume)).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(nextBattle.Team1.Nume))
                    {
                        nextBattle.Team1 = TournamentLiveGame.Team1;
                    }

                    else if (string.IsNullOrWhiteSpace(nextBattle.Team2.Nume))
                    {
                        nextBattle.Team2 = TournamentLiveGame.Team1;
                    }
                }

                if (LiveTournament.IsSemis == true && LiveTournament.IsQuarter == false)
                {
                    if (string.IsNullOrWhiteSpace(LiveTournament.MeciFinal.Team1.Nume))
                    {
                        LiveTournament.MeciFinal.Team1 = TournamentLiveGame.Team1;
                    }

                    else if (string.IsNullOrWhiteSpace(LiveTournament.MeciFinal.Team2.Nume))
                    {
                        LiveTournament.MeciFinal.Team2 = TournamentLiveGame.Team1;
                    }
                }
            }
            else
            {
                TournamentLiveGame.Team2.HasWon = true;
                TournamentLiveGame.Team2.PrevX = TournamentLiveGame.Team2.Scor;
                if (LiveTournament.IsQuarter == true)
                {

                    var nextBattle = LiveTournament.MeciuriSemiFinale
                         .Where(x => string.IsNullOrWhiteSpace(x.Team1.Nume) || string.IsNullOrWhiteSpace(x.Team2.Nume)).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(nextBattle.Team1.Nume))
                    {
                        nextBattle.Team1 = TournamentLiveGame.Team2;
                    }

                    else if (string.IsNullOrWhiteSpace(nextBattle.Team2.Nume))
                    {
                        nextBattle.Team2 = TournamentLiveGame.Team2;
                    }
                }

                if (LiveTournament.IsSemis == true && LiveTournament.IsQuarter == false)
                {
                    if (string.IsNullOrWhiteSpace(LiveTournament.MeciFinal.Team1.Nume))
                    {
                        LiveTournament.MeciFinal.Team1 = TournamentLiveGame.Team2;
                    }

                    else if (string.IsNullOrWhiteSpace(LiveTournament.MeciFinal.Team2.Nume))
                    {
                        LiveTournament.MeciFinal.Team2 = TournamentLiveGame.Team2;
                    }
                }
            }

            if (LiveTournament.IsQuarter == true)
            {
                TournamentLiveGame = LiveTournament.MeciuriSferturi.Where(x => x.Team1.HasWon == false && x.Team2.HasWon == false).FirstOrDefault();
            }

            if (TournamentLiveGame == null && LiveTournament.IsQuarter == true)
            {
                LiveTournament.IsQuarter = false;
                foreach (var meci in LiveTournament.MeciuriSemiFinale)
                {
                    meci.Team1.HasWon = false;
                    meci.Team2.HasWon = false;
                    TournamentLiveGame = LiveTournament.MeciuriSemiFinale.Where(x => x.Team1.HasWon == false && x.Team2.HasWon == false).FirstOrDefault();
                }
            }

            if (LiveTournament.IsQuarter == false)
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

            if (LiveTournament.IsSemis || LiveTournament.IsQuarter || LiveTournament.IsFinal)
            {
                if (TournamentLiveGame.Team1.Payout.Any(x => x.Payout == 0))
                {
                    SaveInPlayGame(new InPlayGame
                    {
                        Game = GetGame(TournamentLiveGame.Team1.Nume),
                        InHuntNumber = 0
                    });
                }
                else
                {
                    SaveInPlayGame(new InPlayGame
                    {
                        Game = GetGame(TournamentLiveGame.Team2.Nume),
                        InHuntNumber = 0
                    });
                }
            }


            SaveCurrentTournament(LiveTournament);
            SaveCurrentTournamentLiveGame(TournamentLiveGame);
        }

        public void UpdateTournament(Tournament tournamenInfo)
        {
            SaveCurrentTournament(tournamenInfo);
            SaveCurrentTournamentLiveGame(tournamenInfo.MeciuriSferturi[0]);
        }

        private void SaveCurrentBetting(BettingModel bettingModel)
        {
            string file = ProjectSettings.LiveBetting;
            database.WriteFile(file, JsonConvert.SerializeObject(bettingModel));
        }

        private void SaveTranzactii(Tranzactii tranzactii)
        {
            string file = ProjectSettings.TranzactiiFile;
            database.WriteFile(file, JsonConvert.SerializeObject(tranzactii));
        }

        private void SaveCurrentBonusHunt(BonusHuntFullInfo bonusHuntFullInfo)
        {
            foreach(var bonus in bonusHuntFullInfo.Bonuses)
            {
                if (bonus.Payed > 0)
                {
                    SaveRound(bonusHuntFullInfo.HuntNumber, bonus);
                }
            }

            string file = ProjectSettings.LiveBonusHuntFile;
            database.WriteFile(file, JsonConvert.SerializeObject(bonusHuntFullInfo));
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


            if (game.Rounds.Any(x=>x.BonusHuntId == huntNumber))
            {
                //update
                var _round = round;

            }
            else
            {
                game.Rounds.Add(round);
            }

            SaveRounds(bonus.GameName,game.Rounds);
            //create
        }

        private void SaveRounds(string gameName, List<Round> rounds)
        {
            var fileName = string.Concat(gameName.Split(Path.GetInvalidFileNameChars()));            
            string file = ProjectSettings.RoundsFolder + fileName.Trim().ToLower() + ".json";
            database.WriteFile(file, JsonConvert.SerializeObject(rounds));
        }

        private void SaveInPlayGame(InPlayGame inPlayGame)
        {
            string file = ProjectSettings.InPlayGame;
            database.WriteFile(file, JsonConvert.SerializeObject(inPlayGame));
        }

        private void SaveCurrentTournamentLiveGame(Meci bonusHuntGridObject)
        {
            string file = ProjectSettings.TournamentLiveGameFile;
            database.WriteFile(file, JsonConvert.SerializeObject(bonusHuntGridObject));
        }

        private void SaveCurrentTournament(Tournament tournament)
        {
            string file = ProjectSettings.TournamentFile;
            database.WriteFile(file, JsonConvert.SerializeObject(tournament));
        }

        //private void AddGame(Game game)
        //{
        //    string file = ProjectSettings.GamesFile;
        //    database.WriteFile(file, JsonConvert.SerializeObject(game));
        //}

        //private void AddProvider(Provider provider)
        //{
        //    string file = ProjectSettings.ProvidersFile;
        //    database.WriteFile(file, JsonConvert.SerializeObject(provider));
        //}
    }
}
