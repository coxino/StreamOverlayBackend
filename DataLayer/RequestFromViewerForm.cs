using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class RequestFromViewerForm : IEquatable<RequestFromViewerForm>
    {
        public RequestFromViewerForm()
        {
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public string Reason { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as RequestFromViewerForm);
        }

        public bool Equals(RequestFromViewerForm other)
        {
            return !(other is null) &&
                   Key == other.Key &&
                   Value == other.Value &&
                   Reason == other.Reason;
        }

        public override int GetHashCode()
        {
            int hashCode = 1423000831;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Key);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Reason);
            return hashCode;
        }
    }

}
