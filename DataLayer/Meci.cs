using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Meci : IEquatable<Meci>
    {
        public Meci()
        {
        }


        public Meci(TournamentCreateInfos json)
        {
            Team1 = new TeamStats(json);
            Team2 = new TeamStats(json);
        }

        public TeamStats Team1 { get; set; }
        public TeamStats Team2 { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Meci);
        }

        public bool Equals(Meci other)
        {
            return other != null &&
                   EqualityComparer<TeamStats>.Default.Equals(Team1, other.Team1) &&
                   EqualityComparer<TeamStats>.Default.Equals(Team2, other.Team2);
        }

        public override int GetHashCode()
        {
            int hashCode = 1071742969;
            hashCode = hashCode * -1521134295 + EqualityComparer<TeamStats>.Default.GetHashCode(Team1);
            hashCode = hashCode * -1521134295 + EqualityComparer<TeamStats>.Default.GetHashCode(Team2);
            return hashCode;
        }
    }
}
