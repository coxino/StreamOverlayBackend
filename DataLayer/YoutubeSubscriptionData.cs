using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class YoutubeSubscriptionData : IEquatable<YoutubeSubscriptionData>
    {
        public YoutubeSubscriptionData()
        {
        }

        public List<YTMember> Members { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as YoutubeSubscriptionData);
        }

        public bool Equals(YoutubeSubscriptionData other)
        {
            return !(other is null) &&
                   EqualityComparer<List<YTMember>>.Default.Equals(Members, other.Members);
        }

        public override int GetHashCode()
        {
            return -1792241104 + EqualityComparer<List<YTMember>>.Default.GetHashCode(Members);
        }
    }

    public class YTMember : IEquatable<YTMember>
    {
        public YTMember()
        {
        }

        public string MemberTier { get; set; }
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public DateTime MemberSince { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as YTMember);
        }

        public bool Equals(YTMember other)
        {
            return !(other is null) &&
                   MemberTier == other.MemberTier &&
                   MemberId == other.MemberId &&
                   MemberName == other.MemberName &&
                   MemberSince == other.MemberSince;
        }

        public override int GetHashCode()
        {
            int hashCode = 384746100;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MemberTier);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MemberId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MemberName);
            hashCode = hashCode * -1521134295 + MemberSince.GetHashCode();
            return hashCode;
        }
    }
}
