using System;
using System.Collections.Generic;

namespace DataLayer
{
    public class BonusHuntPreInfo : IEquatable<BonusHuntPreInfo>
    {
        public BonusHuntPreInfo()
        {
            AverageBet = "0";
            TotalLoss = "0";
            AverageWin = "0";
            AverageMulti = "0";
            AverageMultiToBreakEven = "0";
            BestPayValue = "0";
            Hunting = false;
        }

        public string AverageBet { get; set; }
        public string TotalLoss { get; set; }
        public string AverageWin { get; set; }
        public string AverageMulti { get; set; }
        public string AverageMultiToBreakEven { get; set; }
        public string BestPayGame { get; set; }
        public string BestPayValue { get; set; }
        public bool Hunting { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as BonusHuntPreInfo);
        }

        public bool Equals(BonusHuntPreInfo other)
        {
            return other != null &&
                   AverageBet == other.AverageBet &&
                   TotalLoss == other.TotalLoss &&
                   AverageWin == other.AverageWin &&
                   AverageMulti == other.AverageMulti &&
                   AverageMultiToBreakEven == other.AverageMultiToBreakEven &&
                   BestPayGame == other.BestPayGame &&
                   BestPayValue == other.BestPayValue &&
                   Hunting == other.Hunting;
        }

        public override int GetHashCode()
        {
            int hashCode = 857154216;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AverageBet);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TotalLoss);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AverageWin);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AverageMulti);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AverageMultiToBreakEven);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BestPayGame);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BestPayValue);
            hashCode = hashCode * -1521134295 + Hunting.GetHashCode();
            return hashCode;
        }
    }
}