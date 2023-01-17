using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
  public  class LocalUser : IEquatable<LocalUser>
    {
        public LocalUser()
        {
        }

        public string userIP { get; set; }
        public string superbetName { get; set; }
        public bool isSubscribed { get; set; }
        public string userYoutubeID { get; set; }
        public string userEmail { get; set; }
        public string userName { get; set; }
        public bool isActive { get; set; }
        public int coxiPoints { get; set; }
        public string photoURL { get; set; }
        public string userEmailSecundar { get; set; }
        public string authToken { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as LocalUser);
        }

        public bool Equals(LocalUser other)
        {
            return other != null &&
                   userIP == other.userIP &&
                   superbetName == other.superbetName &&
                   isSubscribed == other.isSubscribed &&
                   userYoutubeID == other.userYoutubeID &&
                   userEmail == other.userEmail &&
                   userName == other.userName &&
                   isActive == other.isActive &&
                   coxiPoints == other.coxiPoints &&
                   photoURL == other.photoURL &&
                   userEmailSecundar == other.userEmailSecundar;
        }

        public override int GetHashCode()
        {
            int hashCode = -512010210;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(userIP);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(superbetName);
            hashCode = hashCode * -1521134295 + isSubscribed.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(userYoutubeID);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(userEmail);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(userName);
            hashCode = hashCode * -1521134295 + isActive.GetHashCode();
            hashCode = hashCode * -1521134295 + coxiPoints.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(photoURL);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(userEmailSecundar);
            return hashCode;
        }
    }
}
