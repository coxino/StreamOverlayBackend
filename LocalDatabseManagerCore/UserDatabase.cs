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
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VybmFtZSI6ImNveGlubyAgICAiLCJQYXNzd29yZCI6IlczeU12SHlUNzQ1YVpuYUMiLCJVc2VySWQiOiI3OTYzZmYwOC04OGU2LTRjZTUtOGI0Zi1mN2MwYmNiOWU3ODMiLCJuYmYiOjE2NTk0NzQyNzUsImV4cCI6MTY2MDA3OTA3NSwiaWF0IjoxNjU5NDc0Mjc1LCJpc3MiOiJodHRwOi8vbXlzaXRlLmNvbSIsImF1ZCI6Imh0dHA6Ly9teWF1ZGllbmNlLmNvbSJ9.DejS2cDdqQnnjTCD0DnJiqssaYd3VhqfHntGtpYDtpk";
            return await new Database().LoginUserAsync(token, context);
        }
    }
}
