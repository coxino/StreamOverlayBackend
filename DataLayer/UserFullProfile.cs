using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class UserFullProfile
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Coins { get; set; }
        public List<RequestFromViewerForm> UserSettings {get; set;}        
    }
}
