using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class ProfitAnimation : IEquatable<ProfitAnimation>
    {
        public bool BonusHundEndOnProfit { get; set; }
        public bool Profit { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ProfitAnimation);
        }

        public bool Equals(ProfitAnimation other)
        {
            return other != null &&
                   BonusHundEndOnProfit == other.BonusHundEndOnProfit &&
                   Profit == other.Profit;
        }

        public override int GetHashCode()
        {
            int hashCode = -824002476;
            hashCode = hashCode * -1521134295 + BonusHundEndOnProfit.GetHashCode();
            hashCode = hashCode * -1521134295 + Profit.GetHashCode();
            return hashCode;
        }
    }
    public class CustomAnimations : IEquatable<CustomAnimations>
    {
        public CustomAnimations()
        {
            ProfitAnimation = new ProfitAnimation();
        }

        public ProfitAnimation ProfitAnimation { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as CustomAnimations);
        }

        public bool Equals(CustomAnimations other)
        {
            return other != null &&
                   EqualityComparer<ProfitAnimation>.Default.Equals(ProfitAnimation, other.ProfitAnimation);
        }

        public override int GetHashCode()
        {
            return -1413785497 + EqualityComparer<ProfitAnimation>.Default.GetHashCode(ProfitAnimation);
        }
    }
}
