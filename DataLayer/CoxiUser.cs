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

        public override bool Equals(object obj)
        {
            return Equals(obj as CoxiUser);
        }

        public bool Equals(CoxiUser other)
        {
            return other != null &&
                   NumeSuperbet == other.NumeSuperbet &&
                   CoxiCoins == other.CoxiCoins;
        }

        public override int GetHashCode()
        {
            int hashCode = 105980798;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NumeSuperbet);
            hashCode = hashCode * -1521134295 + CoxiCoins.GetHashCode();
            return hashCode;
        }
    }
}
