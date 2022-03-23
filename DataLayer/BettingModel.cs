using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class BettingModel
    {
        public BettingModel()
        {
        }

        public BettingModel(int noInGrid,int maxBet)
        {
            MaxBet = maxBet;
            Key = char.ConvertFromUtf32(97 + noInGrid);
            Nume = "";
            Optiune = "!bet " + Key + " " + MaxBet;
            TotalPariat = 0;
        }

        public string Nume { get; set; }
        public string Optiune { get; set; }
        public int MaxBet { get; set; }
        public int Progress { get; set; }
        public int Voturi { get; set; }
        public int TotalPariat { get; set; }
        public string Key { get; set; }
    }
}
