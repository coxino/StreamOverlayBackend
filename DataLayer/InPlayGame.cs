using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class InPlayGame : IEquatable<InPlayGame>
    {
        public InPlayGame()
        {
            Game = new Game();
        }

        public string InHuntNumber { get; set; }
        public Game Game { get; set; }
        public BonusHuntPreInfo BonusHuntPreInfo { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as InPlayGame);
        }

        public bool Equals(InPlayGame other)
        {
            return other != null &&
                   InHuntNumber == other.InHuntNumber &&
                   EqualityComparer<Game>.Default.Equals(Game, other.Game) &&
                   EqualityComparer<BonusHuntPreInfo>.Default.Equals(BonusHuntPreInfo, other.BonusHuntPreInfo);
        }

        public override int GetHashCode()
        {
            int hashCode = 805951151;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(InHuntNumber);
            hashCode = hashCode * -1521134295 + EqualityComparer<Game>.Default.GetHashCode(Game);
            hashCode = hashCode * -1521134295 + EqualityComparer<BonusHuntPreInfo>.Default.GetHashCode(BonusHuntPreInfo);
            return hashCode;
        }
    }
}
