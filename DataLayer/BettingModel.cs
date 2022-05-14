using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class BettingOptionModel
    {
        public BettingOptionModel()
        {

        }

        public BettingOptionModel(int noInGrid, int maxBet)
        {            
            Key = char.ConvertFromUtf32(97 + noInGrid);
            Nume = "";
            Optiune = "!bet " + Key + " " + maxBet;
            TotalPariat = 0;
            IsVisible = true;
            DidRefreshed = false;
        }

        public string Nume { get; set; }
        public string Optiune { get; set; }
        public int Progress { get; set; }
        public int Voturi { get; set; }
        public int TotalPariat { get; set; }
        public string Key { get; set; }
        public bool IsVisible { get; set; }
        public bool DidRefreshed { get; set; }

    }

    public class BettingModel
    {
        public BettingModel()
        {
            Options = new List<BettingOptionModel>();
        }

        public string VoteTitle { get; set; }
        public int MaxBet { get; set; }
        public List<BettingOptionModel> Options { get; set; }
    }
}
