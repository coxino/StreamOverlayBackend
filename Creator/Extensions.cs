﻿using DataLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CreatorDeprecated
{
    public static class Extensions
    {
        //public static T[] Populate<T>(this T[] array, Func<T> provider, int maxBet)
        //{
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        array[i] = provider();
        //    }
        //    return array;
        //}

        public static BettingModel[] Populate(this BettingModel[] array,int maxBet, bool v = false)
        {
            List<string> allNames = LocalDatabaseManager.Database.LiveTournament.MeciuriSferturi.Select(x => x.Team1.Nume).ToList();
            allNames.AddRange(LocalDatabaseManager.Database.LiveTournament.MeciuriSferturi.Select(x => x.Team2.Nume).ToList());
            for (int i = 0; i < array.Length; i++)
            {
                if (v == false)
                {
                    array[i] = new BettingModel(i, maxBet);
                }
                else
                {
                    array[i] = new BettingModel(i, maxBet)
                    {
                        Nume = allNames[i],
                    };
                }
            }
            return array;
        }
    }
}
