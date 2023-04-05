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

    /// <summary>
    /// RAM BASED MEMMORY MAYBE ADD A INTERFACE TO CONTAIN THE DICT LIKE FinalBallance
    /// </summary>
    public class FinalBalance
    {
        private static Dictionary<Guid, FinalBalance> _fFinalBallance;
        private static Dictionary<Guid, FinalBalance> FinalBallance
        {
            get
            {
                _fFinalBallance ??= new Dictionary<Guid, FinalBalance>();
                return _fFinalBallance;
            }
        }

        public static FinalBalance GetUserFinalBallanceVotes(Guid streamerID)
        {
            if(FinalBallance.Keys.Contains(streamerID))
            return FinalBallance.FirstOrDefault(x => x.Key == streamerID).Value;
            else
            {
                var fb = new FinalBalance();
                FinalBallance.Add(streamerID, fb);
                return fb;
            }
        }

        public Dictionary<string, int> BalantaFinala = new Dictionary<string, int>();


        public List<string> Users = new List<string>();
        public Gamequal CurentUser { get; set; }

        public string AdaugaUser(string userID,string username)
        {
            var signup = userID;
            if (Users.Contains(userID))
                return $"@{username} esti inscris deja, nu mai spama!";

            Users.Add(userID);

            return $"@{username} felicitari acum participi la giveaway!";
        }

        public bool AdaugaBalanta(string userID, int ammount)
        {
            if (BalantaFinala.ContainsKey(userID))
                return false;

            BalantaFinala.Add(userID, ammount);

            return true;
        }

        public string CastigatorBalantaFinala(int ammount)
        {
            var closest =  BalantaFinala.OrderBy(n => Math.Abs(n.Value - ammount)).First();

            return closest.Key;
        }

        public string ExtrageUnCastigator()
        {
            int winNO = new Random().Next(0, Users.Count);
            var winName = Users[winNO];

            Users.Clear();

            return winName;
        }
    }
}
