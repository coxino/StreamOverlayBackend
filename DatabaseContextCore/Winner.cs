using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseContext
{
    public class Winner : IEquatable<Winner>
    {
        private int id;
        private int giveawayID;
        private string viewerID;

        public Winner()
        {
        }

        public int Id { get => id; set => id = value; }
        public int GiveawayID { get => giveawayID; set => giveawayID = value; }
        public string ViewerID { get => viewerID; set => viewerID = value; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Winner);
        }

        public bool Equals(Winner other)
        {
            return other != null &&
                   id == other.id &&
                   giveawayID == other.giveawayID &&
                   viewerID == other.viewerID &&
                   Id == other.Id &&
                   GiveawayID == other.GiveawayID &&
                   ViewerID == other.ViewerID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, giveawayID, viewerID, Id, GiveawayID, ViewerID);
        }
    }
}
