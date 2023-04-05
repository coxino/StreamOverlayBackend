using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class StreamerSettings
    {
        public string ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string ProfilePicture { get; set; }  
        public string BackgroundImage { get; set; }
        public LoyaltySettings LoyaltySettings { get; set; }
        public List<RequestFromViewerForm> ShopSettings { get; set; }

    }

    public class LoyaltySettings
    {
        public string LoyaltyName { get; set; }
        public int RewardPerMinute { get; set; }

    }
}
