using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class BonusHuntGridObject : IEquatable<BonusHuntGridObject>
    {
        public BonusHuntGridObject()
        {
            BetSize = 0;
            Payed = 0;
            
        }

        public string PlayerName { get; set; }

        public string GameName { get; set; }

        public string ProviderName { get; set; }

        public double BetSize { get; set; }

        public double Payed { get; set; }

        public bool Disabled { get; set; }

        public double Multiplier
        {
            get
            {
                double bm =  Math.Round(Payed / BetSize, 2);
                if (double.IsNaN(bm) == false)
                    return bm;
                else
                    return 0;
            }
        }

        public bool IsCurrent { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as BonusHuntGridObject);
        }

        public bool Equals(BonusHuntGridObject other)
        {
            return other != null &&
                   PlayerName == other.PlayerName &&
                   GameName == other.GameName &&
                   ProviderName == other.ProviderName &&
                   BetSize == other.BetSize &&
                   Payed == other.Payed &&
                   Disabled == other.Disabled &&
                   Multiplier == other.Multiplier &&
                   IsCurrent == other.IsCurrent;
        }

        public override int GetHashCode()
        {
            int hashCode = 1400080776;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PlayerName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(GameName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProviderName);
            hashCode = hashCode * -1521134295 + BetSize.GetHashCode();
            hashCode = hashCode * -1521134295 + Payed.GetHashCode();
            hashCode = hashCode * -1521134295 + Disabled.GetHashCode();
            hashCode = hashCode * -1521134295 + Multiplier.GetHashCode();
            hashCode = hashCode * -1521134295 + IsCurrent.GetHashCode();
            return hashCode;
        }
    }
}
