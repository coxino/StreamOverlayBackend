using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class ShopItem : IEquatable<ShopItem>
    {
        public ShopItem()
        {
        }

        public ItemType ItemType { get; set; }
        public string ItemID { get; set; }
        public string Nume { get; set; }
        public int Pret { get; set; }
        public string Imagine { get; set; }
        public string Descriere { get; set; }
        public int Stoc { get; set; }
        public bool RequireAditional { get; set; }
        public bool OnlyMembers { get; set; }
        public bool IsVisible { get; set; }
        public double Cooldown { get; set; }
        public string OptionalData { get; set; }

        public List<DropItem> Drops { get; set;}

        public override bool Equals(object obj)
        {
            return Equals(obj as ShopItem);
        }

        public bool Equals(ShopItem other)
        {
            return other != null &&
                   ItemID == other.ItemID &&
                   Nume == other.Nume &&
                   Pret == other.Pret &&
                   Imagine == other.Imagine &&
                   Descriere == other.Descriere &&
                   Stoc == other.Stoc &&
                   RequireAditional == other.RequireAditional &&
                   OnlyMembers == other.OnlyMembers &&
                   IsVisible == other.IsVisible &&
                   Cooldown == other.Cooldown &&
                   OptionalData == other.OptionalData;
        }

        public override int GetHashCode()
        {
            int hashCode = 1483826258;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ItemID);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nume);
            hashCode = hashCode * -1521134295 + Pret.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Imagine);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Descriere);
            hashCode = hashCode * -1521134295 + Stoc.GetHashCode();
            hashCode = hashCode * -1521134295 + RequireAditional.GetHashCode();
            hashCode = hashCode * -1521134295 + OnlyMembers.GetHashCode();
            hashCode = hashCode * -1521134295 + IsVisible.GetHashCode();
            hashCode = hashCode * -1521134295 + Cooldown.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(OptionalData);
            return hashCode;
        }
    }

    public enum ItemType
    {
        NormalItem = 0,
        MysteryBox = 1
    }

    public enum DropType
    {
        Normal = 0,
        Code = 1,
        LoyaltyPoints = 2
    }

    public class DropItem : IEquatable<DropItem>
    {
        public DropItem()
        {
        }

        public string Name { get; set; }
        public int Luck { get; set; }
        public DropType DropType { get; set; }

        public List<string> DropList { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as DropItem);
        }

        public bool Equals(DropItem other)
        {
            return !(other is null) &&
                   Name == other.Name &&
                   Luck == other.Luck &&
                   DropType == other.DropType &&
                   EqualityComparer<List<string>>.Default.Equals(DropList, other.DropList);
        }

        public override int GetHashCode()
        {
            int hashCode = 660267070;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Luck.GetHashCode();
            hashCode = hashCode * -1521134295 + DropType.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(DropList);
            return hashCode;
        }
    }
}
