using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class SubscriptionModel : IEquatable<SubscriptionModel>
    {
        public DateTime EndsOn;

        public SubscriptionModel()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SubscriptionModel);
        }

        public bool Equals(SubscriptionModel other)
        {
            return other != null &&
                   EndsOn == other.EndsOn;
        }

        public override int GetHashCode()
        {
            return 1583838950 + EndsOn.GetHashCode();
        }
    }
}
