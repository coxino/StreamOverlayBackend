using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class UserLoyal : IEquatable<UserLoyal>
    {
        public string id { get; set; }
        public string username { get; set; }
        public int inventory { get; set; }
        public string email { get; set; }
        public string ipadress { get; set; }
        public DateTime creationTime { get; set; }

        public UserLoyal()
        {
            this.id = "";
            this.username = "";
            this.inventory = 0;
            this.email = "";
            this.ipadress = "";
            this.creationTime = DateTime.Now;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as UserLoyal);
        }

        public bool Equals(UserLoyal other)
        {
            return other != null &&
                   id == other.id &&
                   username == other.username &&
                   inventory == other.inventory &&
                   email == other.email &&
                   ipadress == other.ipadress &&
                   creationTime == other.creationTime;
        }

        public override int GetHashCode()
        {
            int hashCode = -1568896010;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(username);
            hashCode = hashCode * -1521134295 + inventory.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(email);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ipadress);
            hashCode = hashCode * -1521134295 + creationTime.GetHashCode();
            return hashCode;
        }
    }

    public class LoyalityRanking : IEquatable<LoyalityRanking>
    {
       public List<UserLoyal> Users { get; set; }

        public LoyalityRanking()
        {
            Users = new List<UserLoyal>();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LoyalityRanking);
        }

        public bool Equals(LoyalityRanking other)
        {
            return other != null &&
                   EqualityComparer<List<UserLoyal>>.Default.Equals(Users, other.Users);
        }

        public override int GetHashCode()
        {
            return 471895467 + EqualityComparer<List<UserLoyal>>.Default.GetHashCode(Users);
        }
    }

    public class LoyalityTiketsRanking
    {
        public List<UserLoyal> Users;

        public LoyalityTiketsRanking()
        {
            Users = new List<UserLoyal>();
        }
    }
}