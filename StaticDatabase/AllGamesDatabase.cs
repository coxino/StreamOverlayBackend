using DataLayer;
using Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StaticDatabase
{
    public static class AllGamesDatabase
    {
        private static List<Game> allGames = null;
        public static List<Game> AllGames
        {
            get
            {
                if(allGames == null)
                {
                    allGames = new List<Game>();
                    if (File.Exists(ProjectSettings.DatabaseFolder + ProjectSettings.AllGamesOnDebug))
                    {
                        allGames = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Game>>(File.ReadAllText(ProjectSettings.DatabaseFolder + ProjectSettings.AllGamesOnDebug));
                    }
                }

                return allGames;
            }
        }

        //public static string BuyGame(string playerName, string gameName, out bool bought)
        //{
        //    var game = AllGames.FirstOrDefault(x => x.Game.Name == gameName);
        //    if(string.IsNullOrWhiteSpace(game.PlayerName))
        //    {
        //        game.PlayerName = playerName;
        //        File.WriteAllText(@"C:\API\database\CupaRomaniei.json", Newtonsoft.Json.JsonConvert.SerializeObject(AllGames));
        //        bought = true;
        //        return "Ai achizitionat cu success jocul " + game.Game.Name;
        //    }
        //    else
        //    {
        //        bought = false;
        //        return "Acest joc a fost cumparat de altcineva!";
        //    }
        //}

        //public static void SaveGames()
        //{
        //    File.WriteAllText(@"C:\API\database\CupaRomaniei.json", Newtonsoft.Json.JsonConvert.SerializeObject(AllGames));
        //}

        //public static object SellGame(string name, string gameName, out bool sold)
        //{
        //    var game = AllGames.FirstOrDefault(x => x.Game.Name == gameName);
        //    if (game.PlayerName == name && game.Bet == 0)
        //    {
        //        game.PlayerName = "";
        //        File.WriteAllText(@"C:\API\database\CupaRomaniei.json", Newtonsoft.Json.JsonConvert.SerializeObject(AllGames));
        //        sold = true;
        //        return "Ai vandut cu success jocul " + game.Game.Name + " acum poti cumpara alt joc!";
        //    }
        //    else
        //    {
        //        sold = false;
        //        return "Din pacate nu ai putut sa vinzi jocul! Probabil a fost deja jucat!";
        //    }
        //}
    }
}
