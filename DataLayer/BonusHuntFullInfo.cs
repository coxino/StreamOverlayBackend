using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class BonusHuntFullInfo : IEquatable<BonusHuntFullInfo>
    {
        public BonusHuntFullInfo()
        {
            Bonuses = new List<BonusHuntGridObject>();
        }

        public int HuntNumber { get; set; }
        public int HuntValue { get; set; }
        public List<BonusHuntGridObject> Bonuses { get; set; }
        public bool BonusHuntEnd { get; set; }
        public int SliceIndex { get; set; }
        public bool IsScrolling { get; set; }
        public bool IsHunting { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as BonusHuntFullInfo);
        }

        public bool Equals(BonusHuntFullInfo other)
        {
            return other != null &&
                   HuntNumber == other.HuntNumber &&
                   HuntValue == other.HuntValue &&
                   EqualityComparer<List<BonusHuntGridObject>>.Default.Equals(Bonuses, other.Bonuses) &&
                   BonusHuntEnd == other.BonusHuntEnd &&
                   SliceIndex == other.SliceIndex &&
                   IsScrolling == other.IsScrolling &&
                   IsHunting == other.IsHunting;
        }

        public override int GetHashCode()
        {
            int hashCode = -1911943793;
            hashCode = hashCode * -1521134295 + HuntNumber.GetHashCode();
            hashCode = hashCode * -1521134295 + HuntValue.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<BonusHuntGridObject>>.Default.GetHashCode(Bonuses);
            hashCode = hashCode * -1521134295 + BonusHuntEnd.GetHashCode();
            hashCode = hashCode * -1521134295 + SliceIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + IsScrolling.GetHashCode();
            hashCode = hashCode * -1521134295 + IsHunting.GetHashCode();
            return hashCode;
        }
    }
}
