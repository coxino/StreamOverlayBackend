using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseContext
{
    public class GivewayTiket : IEquatable<GivewayTiket>
    {
        public GivewayTiket()
        {
        }

        public int Id { get; set; }
        public int GiveawayID { get; set; }
        public string ViewerID { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as GivewayTiket);
        }

        public bool Equals(GivewayTiket other)
        {
            return other != null &&
                   Id == other.Id &&
                   GiveawayID == other.GiveawayID &&
                   ViewerID == other.ViewerID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, GiveawayID, ViewerID);
        }
    }

    public class GivewayModel : IEquatable<GivewayModel>
    {
        public GivewayModel()
        {
        }
        public string OwnerId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EndTime { get; set; }
        public int WinnersCount { get; set; }
        public int Price { get; set; }
        public int MaxTikets { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as GivewayModel);
        }

        public bool Equals(GivewayModel other)
        {
            return other != null &&
                   Id == other.Id &&
                   Name == other.Name &&
                   Description == other.Description &&
                   EndTime == other.EndTime &&
                   WinnersCount == other.WinnersCount &&
                   Price == other.Price &&
                   MaxTikets == other.MaxTikets;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Description, EndTime, WinnersCount, Price, MaxTikets);
        }
    }
}
