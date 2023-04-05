using DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalDatabaseManager
{
    public class UserDatabase
    {
        public static async Task<Database> GetDatabaseAsync(string token, ApplicationDbContext dbContext)
        {
            return await new Database().LoginUserAsync(token,dbContext);
        }

        public static Database GetByUsername(string username)
        {
            return new Database(username);
        }

        /// <summary>
        /// Resolve this, can cause issues
        /// </summary>
        /// <param name="context"></param>
        /// <param name="streamerId"></param>
        /// <returns></returns>
        public static async Task<Database> GetGivewayDBAsync(ApplicationDbContext context, string streamerId = "")
        {            
            return new Database().LoginUserReadOnlyAsync(streamerId, context);
        }
    }
}
