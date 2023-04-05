using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Redeem
    {
        public string UserId { get; set; }
        public string TimeOfWin { get; set; }
        public string WinnerName { get; set; }

        public TwitchSubscription TwitchSubscription { get; set; }
        public YTMember MembershipDetails { get; set; }
        public List<RequestFromViewerForm> ViewerSettings { get; set; }
        public ShopItem ShopItem { get; set; }

        public Redeem()
        {
            TimeOfWin = DateTime.Now.ToString("F");
        }
    }
}
