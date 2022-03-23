using DataLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryManipulator
{
    class FileWriter
    {
        internal static bool SaveData(string file, object data, object lockFile)
        {
            if (WriteFile(file, data, lockFile) == false)
            {
                return SaveData(file, data, lockFile);
            }

            return true;
        }

        private static bool WriteFile(string file, object data, object lockFile)
        {
            lock (lockFile)
            {
                try
                {
                    if (!File.Exists(file))
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(file)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(file));
                        }
                        File.WriteAllText(file, "");
                    }
                    File.WriteAllText(file, JsonConvert.SerializeObject(data));
                }
                catch
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public static bool AppendData(string file, object data, object lockFile)
        {
            if (AppendFile(file, data, lockFile) == false)
            {
                return AppendFile(file, data, lockFile);
            }

            return true;
        }


        private static bool AppendFile(string file, object data, object lockFile)
        {
            lock (lockFile)
            {
                try
                {
                    if (!File.Exists(file))
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(file)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(file));
                        }
                        File.WriteAllText(file, "");
                    }
                    File.WriteAllText(file, JsonConvert.SerializeObject(data));
                }
                catch
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
