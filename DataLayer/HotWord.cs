using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class HotWord : IEquatable<HotWord>
    {
        public HotWord()
        {
        }

        public string Word { get; set; }
        public double Degree { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as HotWord);
        }

        public bool Equals(HotWord other)
        {
            return other != null &&
                   Word == other.Word &&
                   Degree == other.Degree;
        }

        public override int GetHashCode()
        {
            int hashCode = 1130289710;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Word);
            hashCode = hashCode * -1521134295 + Degree.GetHashCode();
            return hashCode;
        }
    }
}
