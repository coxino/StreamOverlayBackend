using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class YoutubeEventModel : EventModel, IEquatable<YoutubeEventModel>
    {
        public YoutubeEventModel()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as YoutubeEventModel);
        }

        public bool Equals(YoutubeEventModel other)
        {
            return other != null &&
                   base.Equals(other) &&
                   Service == other.Service &&
                   EqualityComparer<EventData>.Default.Equals(Data, other.Data) &&
                   RenderedText == other.RenderedText;
        }

        public override int GetHashCode()
        {
            int hashCode = -1353651702;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Service);
            hashCode = hashCode * -1521134295 + EqualityComparer<EventData>.Default.GetHashCode(Data);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RenderedText);
            return hashCode;
        }
    }
}
