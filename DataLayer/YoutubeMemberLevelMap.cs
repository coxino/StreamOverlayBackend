using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class YoutubeMemberLevelMap : IEquatable<YoutubeMemberLevelMap>
    {
        public YoutubeMemberLevelMap()
        {
        }

        public string MemberLevelString { get; set; }
        public MemberLevels MemberLevelEnum { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as YoutubeMemberLevelMap);
        }

        public bool Equals(YoutubeMemberLevelMap other)
        {
            return !(other is null) &&
                   MemberLevelString == other.MemberLevelString &&
                   MemberLevelEnum == other.MemberLevelEnum;
        }

        public override int GetHashCode()
        {
            int hashCode = -1437213136;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MemberLevelString);
            hashCode = hashCode * -1521134295 + MemberLevelEnum.GetHashCode();
            return hashCode;
        }
    }
}
