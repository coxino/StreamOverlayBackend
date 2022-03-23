using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalDatabaseManager
{
    public class UserDatabase
    {
        public static Database GetDatabase(string userID)
        {
            //open database atm
            return new Database(userID);
        }
    }
}
