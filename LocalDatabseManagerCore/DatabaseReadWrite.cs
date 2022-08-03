using DataLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LocalDatabaseManager
{
    public class FileLocker
    {
        public string fileName;
    }

    public class DatabaseReadWrite
    {
        private static readonly object SyncRoot = new object();
        private string userID;

        public DatabaseReadWrite(string _userId)
        {
            userID = _userId;
        }

        private string ReadFile(string file)
        {
            lock (SyncRoot)
            {
                file = Settings.ProjectSettings.DatabaseFolder + userID + file;
                if (!File.Exists(file))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(file)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(file));
                    }
                    File.WriteAllText(file, "");
                }
                return File.ReadAllText(file);
            }
        }

        internal string ReadString(string file)
        {
            return ReadFile(file);
        }

        internal T ReadFile<T>(string file)
        {
            lock (SyncRoot)
            {
                string obj = ReadFile(file);
                return JsonConvert.DeserializeObject<T>(obj);
            }
        }

        public void WriteFile(string file, string content)
        {
            lock (SyncRoot)
            {
                file = Settings.ProjectSettings.DatabaseFolder + userID + file;
                if (!File.Exists(file))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(file)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(file));
                    }
                    File.WriteAllText(file, "");
                }
                File.WriteAllText(file, content);
            }
        }

        internal void AppendFile(string file, string content)
        {
            lock (SyncRoot)
            {
                file = Settings.ProjectSettings.DatabaseFolder + userID + file;
                if (!File.Exists(file))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(file)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(file));
                    }
                    File.WriteAllText(file, "");
                }
                File.AppendAllText(file, content);
            }
        }

        internal Game GetGame(string gameName)
        {
            var file = Settings.ProjectSettings.DatabaseFolder + Settings.ProjectSettings.GamesFile;
            lock (SyncRoot)
            {
                if (!File.Exists(file))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(file)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(file));
                    }
                    File.WriteAllText(file, "");
                }
            }
            Game game;

            var Games = JsonConvert.DeserializeObject<List<Game>>(File.ReadAllText(file));
            game = Games?.FirstOrDefault(x => x.Name == gameName) ?? null;

            if(game != null)
            game.Rounds = GameRounds(game.Name);

            return game;
        }

        private List<Round> GameRounds(string gameName)
        {
            var fileName = string.Concat(gameName.Split(Path.GetInvalidFileNameChars()));
            var rounds = ReadFile<List<Round>>(Settings.ProjectSettings.RoundsFolder + fileName.Trim().ToLower() + ".json");
            if (rounds != null)
            {
                return rounds;
            }
            else
            {
                return new List<Round>();
            }
        }

        internal void SaveUserBetsToBetFile(string file, string user, string amount)
        {
            lock (SyncRoot)
            {
                file = Settings.ProjectSettings.DatabaseFolder + userID + file;
                if (!File.Exists(file))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(file)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(file));
                    }
                    File.WriteAllText(file, "");
                }
                File.AppendAllText(file, user + ":" + amount);
            }
        }

        internal int ReadUserJackpot()
        {
            lock (SyncRoot)
            {
                var file = Settings.ProjectSettings.DatabaseFolder + userID + "/" + Settings.ProjectSettings.JackpotFile;
                if (File.Exists(file))
                {
                    int ret = 0;
                    int.TryParse(File.ReadAllText(file), out ret);
                    return ret;

                }

                return 0;
            }
        }

        internal void RemoveBettingFiles()
        {
            Directory.Delete(Settings.ProjectSettings.DatabaseFolder + userID + Settings.ProjectSettings.LiveBettingUserOptions, true);
            File.Delete(Settings.ProjectSettings.DatabaseFolder + userID + Settings.ProjectSettings.CooldownFolder + "bet" + Settings.ProjectSettings.CooldownFile);
        }

        internal void UpdateGame(Game inDbGame, Game game)
        {
            var file = Settings.ProjectSettings.DatabaseFolder + Settings.ProjectSettings.GamesFile;
            var Games = JsonConvert.DeserializeObject<List<Game>>(File.ReadAllText(file));
            game = Games?.FirstOrDefault(x => x.Name == inDbGame.Name);
            game.Image = game.Image;
        }

        internal void SaveNewGame(InPlayGame _game)
        {
            if (_game == null || string.IsNullOrWhiteSpace(_game.Game.Name))
                return;

            var file = Settings.ProjectSettings.DatabaseFolder + Settings.ProjectSettings.GamesFile;
            Game game;

            var Games = JsonConvert.DeserializeObject<List<Game>>(File.ReadAllText(file));

            var gg = Games.Where(x => string.IsNullOrWhiteSpace(x.Image));

            //Games.RemoveAll(x => string.IsNullOrWhiteSpace(x.Image));

            game = Games?.FirstOrDefault(x => x.Name == _game.Game.Name);

            var shouldCreate = game == null;

            if (game != null && game?.Image != _game.Game.Image)
            {
                shouldCreate = true;
                Games.Remove(game);
            }          

            if (shouldCreate)
            {
                game = _game.Game;
                Games.Add(game);
                file = Settings.ProjectSettings.DatabaseFolder + Settings.ProjectSettings.GamesFile;
                lock (SyncRoot)
                {
                    File.WriteAllText(file, JsonConvert.SerializeObject(Games));
                    File.WriteAllText(Settings.ProjectSettings.AllGamesOnDebug, JsonConvert.SerializeObject(Games));
                    File.WriteAllText(Settings.ProjectSettings.AllGamesOnWebsite, JsonConvert.SerializeObject(Games));
                }
            }
        }
    }
}
