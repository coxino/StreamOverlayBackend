using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class CupaRomanieGame : IEquatable<CupaRomanieGame>
    {
        public CupaRomanieGame()
        {
        }

        public string PlayerName { get; set; }
        public Game Game { get; set; }
        public bool Calificat { get; set; }
        public int PayOut { get; set; }
        public double Bet { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as CupaRomanieGame);
        }

        public bool Equals(CupaRomanieGame other)
        {
            return other != null &&
                   PlayerName == other.PlayerName &&
                   EqualityComparer<Game>.Default.Equals(Game, other.Game) &&
                   Calificat == other.Calificat &&
                   PayOut == other.PayOut &&
                   Bet == other.Bet;
        }

        public override int GetHashCode()
        {
            int hashCode = -115051861;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PlayerName);
            hashCode = hashCode * -1521134295 + EqualityComparer<Game>.Default.GetHashCode(Game);
            hashCode = hashCode * -1521134295 + Calificat.GetHashCode();
            hashCode = hashCode * -1521134295 + PayOut.GetHashCode();
            hashCode = hashCode * -1521134295 + Bet.GetHashCode();
            return hashCode;
        }
    }
}
