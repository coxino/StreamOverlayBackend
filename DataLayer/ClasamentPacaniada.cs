using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class UserPacaniada : IEquatable<UserPacaniada>
    {
        public UserPacaniada()
        {
        }

        public string Name { get; set; }
        public double Start { get; set; }
        public double End { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as UserPacaniada);
        }

        public bool Equals(UserPacaniada other)
        {
            return other != null &&
                   Name == other.Name &&
                   Start == other.Start &&
                   End == other.End;
        }

        public override int GetHashCode()
        {
            int hashCode = -1773810425;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Start.GetHashCode();
            hashCode = hashCode * -1521134295 + End.GetHashCode();
            return hashCode;
        }
    }

    public class ClasamentPacaniada
    {
        public List<UserPacaniada> Users { get; set; }
    }
}
