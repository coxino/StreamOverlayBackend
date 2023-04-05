using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class RumbleTeam
    {
        public RumbleTeam()
        {
            GameName = "GameName";
            NumeJucator = "Player";
            BuyCost = 1000;
            Payout = 0;
        }

        public string GameName { get; set; }
        public string NumeJucator { get; set; }
        public double BuyCost { get; set; }
        public double Payout { get; set; }
    }

    public class RumbleMeci
    {
        public RumbleMeci()
        {
            Team1 = new RumbleTeam();
            Team2 = new RumbleTeam();
        }

        public RumbleTeam Team1 { get; set; }
        public RumbleTeam Team2 { get; set; }
    }

    public class SlotsRumbleModel
    {
        public int RumbleId { get; set; }

        public SlotsRumbleModel()
        {
            GameHistory = new List<RumbleMeci>();
        }

        public List<RumbleMeci> GameHistory { get; set; }
    }
}
