using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer
{
    public class Game : IEquatable<Game>
    {
        public Game()
        {
            Rounds = new List<Round>();
        }

        public string Name { get; set; }
        public string Provider { get; set; }
        public string Potential { get; set; } = "";
        public string Volatility { get; set; } = "";
        public string Image { get; set; }
        public List<Round> Rounds { get; set; }
        

        public override bool Equals(object obj)
        {
            return Equals(obj as Game);
        }

        public bool Equals(Game other)
        {
            return other != null &&
                   Name == other.Name &&
                   Provider == other.Provider &&
                   EqualityComparer<List<Round>>.Default.Equals(Rounds, other.Rounds) &&
                   Potential == other.Potential &&
                   Volatility == other.Volatility &&
                   Image == other.Image;
        }

        public override int GetHashCode()
        {
            int hashCode = -1608181101;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Provider);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Round>>.Default.GetHashCode(Rounds);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Potential);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Volatility);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Image);
            return hashCode;
        }
    }
}
