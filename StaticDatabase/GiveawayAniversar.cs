using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Youtube_Contractor;
using System.Linq;

namespace StaticDatabase
{
    public class Gamequal
    {
        public string playerName;
        public string gameName;
    }

    public static class GiveawayAniversar
    {
        public static Dictionary<string, int> BalantaFinala = new Dictionary<string, int>();


        public static List<string> Users = new List<string>();
        public static Gamequal CurentUser { get; set; }

        public static string AdaugaUser(string userID,string username)
        {
            var signup = userID;
            if (Users.Contains(userID))
                return $"@{username} esti inscris deja, nu mai spama!";

            Users.Add(userID);

            return $"@{username} felicitari acum participi la giveaway!";
        }

        public static bool AdaugaBalanta(string userID, int ammount)
        {
            if (BalantaFinala.ContainsKey(userID))
                return false;

            BalantaFinala.Add(userID, ammount);

            return true;
        }

        public static string CastigatorBalantaFinala(int ammount)
        {
            var closest =  BalantaFinala.OrderBy(n => Math.Abs(n.Value - ammount)).First();

            return closest.Key;
        }

        public static string ExtrageUnCastigator()
        {
            int winNO = new Random().Next(0, Users.Count);
            var winName = Users[winNO];

            Users.Clear();

            return winName;
        }
    }
}
