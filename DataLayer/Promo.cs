using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Click
    {
        public Click()
        {
        }

        public DateTime ClickTime { get; set; }
        public string IpAdress { get; set; }
    }

    //lazy?
    public class PromoClicks
    {
        public PromoClicks()
        {
        }

        public string PromoName { get; set; }
        public List<Click> Clicks { get; set; }
    }

    public class Promo : IEquatable<Promo>
    {
        public Promo()
        {
        }

        public string descriere { get; set; }
        public string image { get; set; }
        public string link { get; set; }
        public string name { get; set; }
        public int rating { get; set; }
        public string button { get; set; }
        public string descriereCard { get; set; }
        public string nameCard { get; set; }
        public string command { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Promo);
        }

        public bool Equals(Promo other)
        {
            return !(other is null) &&
                   descriere == other.descriere &&
                   image == other.image &&
                   link == other.link &&
                   name == other.name &&
                   rating == other.rating &&
                   button == other.button;
        }

        public override int GetHashCode()
        {
            int hashCode = 276444875;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(descriere);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(image);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(link);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + rating.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(button);
            return hashCode;
        }
    }
}
