using DataLayer;
using Settings;
using System;
using System.Collections.Generic;
using System.IO;

namespace gamemanager
{
    public class GameInfo
    {
        public string Name { get; set; }
        public string Image { get; set; }
    }

    class Program
    {
        
        static void Main(string[] args)
        {
            List<Game> Games = new List<Game>();
            List<GameInfo> GamesX = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GameInfo>>(File.ReadAllText(@"C:\New folder\allgames.json"));

            foreach(var game in GamesX)
            {
                Games.Add(new Game()
                {
                    Name = game.Name,
                    Potential = "",
                    Provider = "",
                    Rounds = new List<Round>(),
                    Volatility = "",  
                    Image = game.Image
                });
            }

            File.WriteAllText(ProjectSettings.DatabaseFolder + ProjectSettings.GamesFile, Newtonsoft.Json.JsonConvert.SerializeObject(Games));
        }
    }
}
