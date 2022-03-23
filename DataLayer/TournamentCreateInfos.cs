using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class TournamentCreateInfos : IEquatable<TournamentCreateInfos>
    {
        public TournamentCreateInfos()
        {
        }

        public int Fights { get; set; }
        public int BuyValue { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as TournamentCreateInfos);
        }

        public bool Equals(TournamentCreateInfos other)
        {
            return other != null &&
                   Fights == other.Fights &&
                   BuyValue == other.BuyValue;
        }

        public override int GetHashCode()
        {
            int hashCode = 1931299526;
            hashCode = hashCode * -1521134295 + Fights.GetHashCode();
            hashCode = hashCode * -1521134295 + BuyValue.GetHashCode();
            return hashCode;
        }
    }
}
