using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class GiveawayRequestModel : IEquatable<GiveawayRequestModel>
    {
        public GiveawayRequestModel()
        {
        }

        public LocalUser localUser { get; set; }
        public int giveawayId { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as GiveawayRequestModel);
        }

        public bool Equals(GiveawayRequestModel other)
        {
            return other != null &&
                   EqualityComparer<LocalUser>.Default.Equals(localUser, other.localUser) &&
                   giveawayId == other.giveawayId;
        }

        public override int GetHashCode()
        {
            int hashCode = -41633322;
            hashCode = hashCode * -1521134295 + EqualityComparer<LocalUser>.Default.GetHashCode(localUser);
            hashCode = hashCode * -1521134295 + giveawayId.GetHashCode();
            return hashCode;
        }
    }
}
