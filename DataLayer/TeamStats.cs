using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class PayoutTemplate : IEquatable<PayoutTemplate>
    {
        public string n { get; set; }
        public int Payout { get; set; }

        public PayoutTemplate(int i)
        {
            n = (i+1).ToString();
            Payout = 0;            
        }

        public PayoutTemplate()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PayoutTemplate);
        }

        public bool Equals(PayoutTemplate other)
        {
            return other != null &&
                   n == other.n &&
                   Payout == other.Payout;
        }

        public override int GetHashCode()
        {
            int hashCode = 73834364;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(n);
            hashCode = hashCode * -1521134295 + Payout.GetHashCode();
            return hashCode;
        }
    }
    public class TeamStats : IEquatable<TeamStats>
    {
        public TeamStats()
        {
        }

        public TeamStats(TournamentCreateInfos json)
        {
            Payout = new List<PayoutTemplate>();
            for (int i = 0; i < json.Fights; i++)
            {
                Payout.Add(new PayoutTemplate(i));
            }

            BuyCost = json.BuyValue;
            PrevX = 0;
            HasWon = false;
        }

        public TeamStats(TeamStats team1)
        {
            Payout = new List<PayoutTemplate>();
            Nume = team1.Nume;
            NumeJucator = team1.NumeJucator;
            BuyCost = team1.BuyCost;
            for (int i = 0; i < team1.Payout.Count; i++)
            {
                Payout.Add(new PayoutTemplate(i));
            }

            PrevX = 0;
            HasWon = false;
        }

        public string NumeJucator { get; set; }
        public string Nume { get; set; }
        public double BuyCost { get; set; }
        public List<PayoutTemplate> Payout { get; set; }
        public double Scor
        {
            get
            {
                if (Payout != null)
                {
                    var sc = Math.Round(Payout.Sum(x => x.Payout) / BuyCost, 2);
                    if (double.IsNaN(sc))
                        return 0;
                    return sc;
                }
                else return 0;
            }
        }

        public bool HasWon { get; set; }
        public double PrevX { get; set; }
        public bool IsCurrent { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as TeamStats);
        }

        public bool Equals(TeamStats other)
        {
            return other != null &&
                   Nume == other.Nume &&
                   NumeJucator == other.NumeJucator &&
                   BuyCost == other.BuyCost &&
                   EqualityComparer<List<PayoutTemplate>>.Default.Equals(Payout, other.Payout) &&
                   Scor == other.Scor &&
                   HasWon == other.HasWon &&
                   PrevX == other.PrevX &&
                   IsCurrent == other.IsCurrent;
        }

        public override int GetHashCode()
        {
            int hashCode = -1398063904;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nume);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NumeJucator);
            hashCode = hashCode * -1521134295 + BuyCost.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<PayoutTemplate>>.Default.GetHashCode(Payout);
            hashCode = hashCode * -1521134295 + Scor.GetHashCode();
            hashCode = hashCode * -1521134295 + HasWon.GetHashCode();
            hashCode = hashCode * -1521134295 + PrevX.GetHashCode();
            hashCode = hashCode * -1521134295 + IsCurrent.GetHashCode();
            return hashCode;
        }
    }
}
