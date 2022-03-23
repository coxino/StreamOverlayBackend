using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class UserUpdateModel
    {
        public string token { get; set; }
        public string userID { get; set; }
        /// <summary>
        /// USERNAME IS USER NAME NOT VIEWER NAME CAREFULLL
        /// </summary>
        public string username { get; set; }
        public string email { get; set; }
        public string ipadress { get; set; }
        public string itemID { get; set; }
        public string numeSuperbet { get; set; }
        public string numeSpeciala { get; set; }
    }
}
