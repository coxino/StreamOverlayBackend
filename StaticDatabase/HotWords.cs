using DataLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace StaticDatabase
{
    public static class HotWords
    {
      public static Dictionary<string, List<HotWord>> UserWordsPair = new Dictionary<string,List<HotWord>>();

        public static void AddWord(string userId, string bettingOption)
        {
            if (UserWordsPair.ContainsKey(userId))
            {
                if(UserWordsPair[userId].Any(x=>x.Word == bettingOption))
                {
                    UserWordsPair[userId].Where(x => x.Word == bettingOption).FirstOrDefault().Degree++;
                }
                else
                {
                    UserWordsPair[userId].Add(new HotWord() { Degree = 1, Word = bettingOption });
                }
            }
            else
            {
                UserWordsPair.Add(userId, new List<HotWord>());
            }
        }

        public static List<HotWord> GetHotWords(string username)
        {
            if (UserWordsPair.ContainsKey(username) == false)
                return new List<HotWord>();

            var wd = UserWordsPair[username].OrderByDescending(x=>x.Degree).Take(5).ToList();

            return wd;
        }

        public static void ResetHotWords(string v)
        {
            if (UserWordsPair.ContainsKey(v))
            {
                UserWordsPair[v].Clear();
            }
        }
    }
}
