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
}
