﻿using DataLayer;
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
    public class DatabaseReadWrite
    {
        private string userID;

        public DatabaseReadWrite(string _userId)
        {
            userID = _userId;
        }

        private string ReadFile(string file)
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

        internal string ReadString(string file)
        {
            return ReadFile(file);
        }

        internal T ReadFile<T>(string file)
        {
            string obj = ReadFile(file);            
            return JsonConvert.DeserializeObject<T>(obj);
        }

        public void WriteFile(string file, string content)
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

        internal Game GetGame(string gameName)
        {
            var file = Settings.ProjectSettings.DatabaseFolder + Settings.ProjectSettings.GamesFile;
            if (!File.Exists(file))
            {
                if (!Directory.Exists(Path.GetDirectoryName(file)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file));
                }
                File.WriteAllText(file, "");
            }
            Game game;

            var Games = JsonConvert.DeserializeObject<List<Game>>(File.ReadAllText(file));
            game = Games?.FirstOrDefault(x => x.Name == gameName);

            if(game == null) 
            { 
                game = new Game() { Name = gameName };

                if (userID == "coxino")
                {
                    Games.Add(game);
                    file = Settings.ProjectSettings.DatabaseFolder + Settings.ProjectSettings.GamesFile;
                    File.WriteAllText(file, JsonConvert.SerializeObject(Games));
                }
            }

            game.Rounds = GameRounds(gameName);
            
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
    }
}
