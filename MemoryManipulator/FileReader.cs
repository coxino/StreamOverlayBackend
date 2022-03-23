using Newtonsoft.Json;
using Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryManipulator
{
    class FileReader
    {
        internal static string ReadFile(string file)
        {            
            if (!File.Exists(file))
            {
                if (!Directory.Exists(Path.GetDirectoryName(file)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file));
                }
                File.WriteAllText(file, "");
            }
            return File.ReadAllText(file);
        }

        internal static T ReadFile<T>(string file)
        {
            string obj = ReadFile(file);
            return JsonConvert.DeserializeObject<T>(obj);
        }
    }
}
