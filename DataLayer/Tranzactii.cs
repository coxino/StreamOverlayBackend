using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Tranzactii : IEquatable<Tranzactii>
    {
        public Tranzactii()
        {
            Depunere = 0;
            Retragere = 0;
        }

        public Tranzactii(int depunere, int retragere)
        {
            Depunere = depunere;
            Retragere = retragere;
        }

        public int Depunere { get; set; }
        public int Retragere { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Tranzactii);
        }

        public bool Equals(Tranzactii other)
        {
            return other != null &&
                   Depunere == other.Depunere &&
                   Retragere == other.Retragere;
        }

        public override int GetHashCode()
        {
            int hashCode = -2116120703;
            hashCode = hashCode * -1521134295 + Depunere.GetHashCode();
            hashCode = hashCode * -1521134295 + Retragere.GetHashCode();
            return hashCode;
        }
    }
}
