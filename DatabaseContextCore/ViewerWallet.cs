using DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseContextCore
{
    public class ViewerWallet : IEquatable<ViewerWallet>
    {
        public ViewerWallet()
        {
        }

        public int Id { get; set; }
        public string StreamerId { get; set; }
        public string ViewerId { get; set; }      
        public int Coins { get; set; }

        public Viewer Viewer { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ViewerWallet);
        }

        public bool Equals(ViewerWallet other)
        {
            return other is not null &&
                   Id == other.Id &&
                   StreamerId == other.StreamerId &&
                   ViewerId == other.ViewerId &&
                   Coins == other.Coins &&
                   EqualityComparer<Viewer>.Default.Equals(Viewer, other.Viewer);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, StreamerId, ViewerId, Coins, Viewer);
        }
    }
}
