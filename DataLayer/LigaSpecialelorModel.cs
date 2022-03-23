using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Speciala : IEquatable<Speciala>
    {
        public Speciala()
        {
        }

        public string Nume { get; set; }
        public double BuyIn { get; set; }
        public double Payed { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Speciala);
        }

        public bool Equals(Speciala other)
        {
            return other != null &&
                   Nume == other.Nume &&
                   BuyIn == other.BuyIn &&
                   Payed == other.Payed;
        }

        public override int GetHashCode()
        {
            int hashCode = 1169313338;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nume);
            hashCode = hashCode * -1521134295 + BuyIn.GetHashCode();
            hashCode = hashCode * -1521134295 + Payed.GetHashCode();
            return hashCode;
        }
    }
    public class Rank : IEquatable<Rank>
    {
        public Rank()
        {
        }

        public string Nume { get; set; }
        public int Sanse { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Rank);
        }

        public bool Equals(Rank other)
        {
            return other != null &&
                   Nume == other.Nume &&
                   Sanse == other.Sanse;
        }

        public override int GetHashCode()
        {
            int hashCode = -796013753;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nume);
            hashCode = hashCode * -1521134295 + Sanse.GetHashCode();
            return hashCode;
        }
    }

    public class LigaUser : IEquatable<LigaUser>
    {
        public LigaUser()
        {
        }

        public string Nume { get; set; }
        public Rank Rank { get; set; }
        public List<Speciala> Speciale { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as LigaUser);
        }

        public bool Equals(LigaUser other)
        {
            return other != null &&
                   Nume == other.Nume &&
                   EqualityComparer<Rank>.Default.Equals(Rank, other.Rank) &&
                   EqualityComparer<List<Speciala>>.Default.Equals(Speciale, other.Speciale);
        }

        public override int GetHashCode()
        {
            int hashCode = -680262288;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nume);
            hashCode = hashCode * -1521134295 + EqualityComparer<Rank>.Default.GetHashCode(Rank);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Speciala>>.Default.GetHashCode(Speciale);
            return hashCode;
        }
    }
    class LigaSpecialelorModel
    {
    }
}
