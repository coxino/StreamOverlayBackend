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

        public static async Task<Database> GetGivewayDBAsync(ApplicationDbContext context)
        {
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VybmFtZSI6ImNveGlubyAgICAiLCJQYXNzd29yZCI6ImNvc21pbjEyMzQgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIiwibmJmIjoxNjQxNzUxMzQ4LCJleHAiOjE2NDIzNTYxNDgsImlhdCI6MTY0MTc1MTM0OCwiaXNzIjoiaHR0cDovL215c2l0ZS5jb20iLCJhdWQiOiJodHRwOi8vbXlhdWRpZW5jZS5jb20ifQ.mBT7x_HqxVaLqZFLExmMKEp0sLMzlAlECMD-hXAtAuM";
            return await new Database().LoginUserAsync(token, context);
        }
    }
}
