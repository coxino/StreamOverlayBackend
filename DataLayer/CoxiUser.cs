using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class CoxiUser : IEquatable<CoxiUser>
    {
        public CoxiUser()
        {
        }
        public string NumeSuperbet { get; set; }
        public int CoxiCoins { get; set; }
        public bool isActive { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as CoxiUser);
        }

        public bool Equals(CoxiUser other)
        {
            return other != null &&
                   NumeSuperbet == other.NumeSuperbet &&
                   CoxiCoins == other.CoxiCoins &&
                   isActive == other.isActive;
        }

        public override int GetHashCode()
        {
            int hashCode = 808614285;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NumeSuperbet);
            hashCode = hashCode * -1521134295 + CoxiCoins.GetHashCode();
            hashCode = hashCode * -1521134295 + isActive.GetHashCode();
            return hashCode;
        }
    }
}
