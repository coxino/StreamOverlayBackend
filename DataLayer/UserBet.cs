using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class UserBet : IEquatable<UserBet>
    {
        public UserBet()
        {
        }

        public string Name { get; set; }
        public string UserID { get; set; }
        public int Bet { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as UserBet);
        }

        public bool Equals(UserBet other)
        {
            return other != null &&
                   Name == other.Name &&
                   UserID == other.UserID &&
                   Bet == other.Bet;
        }

        public override int GetHashCode()
        {
            int hashCode = 1959565273;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserID);
            hashCode = hashCode * -1521134295 + Bet.GetHashCode();
            return hashCode;
        }
    }
}
