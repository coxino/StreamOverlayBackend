using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer
{
    public class Round
    {
        public int BonusHuntId { get; set; }
        public double BetSize { get; set; }
        public double PayAmount { get; set; }
        public int Multiplier
        {
            get
            {
                return (int)(PayAmount / BetSize);
            }
        }
    }
}
