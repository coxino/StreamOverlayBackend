using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class ShopRequestModel 
    {
        public ShopRequestModel()
        {
        }

        public string localUserToken { get; set; }
        public ShopItem item { get; set; }

    }
}
