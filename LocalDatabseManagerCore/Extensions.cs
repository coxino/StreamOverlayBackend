using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalDatabseManager
{
    public static class Extensions
    {
        public static string ToAscii(this string str)
        {
            StringBuilder sb = new();
            foreach (char c in str)
            {
                if (c < 128)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
