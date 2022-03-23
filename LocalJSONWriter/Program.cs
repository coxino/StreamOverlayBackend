using System;
using System.Collections.Generic;
using System.IO;
using DataLayer;
using Newtonsoft.Json;

namespace LocalJSONWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            ShopItem shopItem = new ShopItem()
            {
                ItemID = "Item1",
                Nume = "100 PSC",
                Descriere = "Participa la extragerea saptamanala",
                Imagine = "",
                Pret = 1000,
                Stoc = 5
            };


            List<ShopItem> shop = new List<ShopItem>();

            shop.Add(shopItem);

            File.WriteAllText(@"C:\API\database\coxino\Shop\shop.json", JsonConvert.SerializeObject(shop));
        }
    }
}