using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class TwitchSubscription
    {
        public string broadcaster_id { get; set; }
        public string broadcaster_login { get; set; }
        public string broadcaster_name { get; set; }
        public string gifter_id { get; set; }
        public string gifter_login { get; set; }
        public string gifter_name { get; set; }
        public bool is_gift { get; set; }
        public string tier { get; set; }
        public string plan_name { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string user_login { get; set; }
    }

    public class Pagination
    {
        public string cursor { get; set; }
    }

    public class TwitchSubscriptionResponse
    {
        public List<TwitchSubscription> data { get; set; }
        public Pagination pagination { get; set; }
        public int total { get; set; }
        public int points { get; set; }
    }
}
