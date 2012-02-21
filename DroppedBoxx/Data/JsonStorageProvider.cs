using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DroppedBoxx.Data
{
    public class JsonStorageProvider
    {
        private static string _configDirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "DroppedBoxx";

        public static void StoreObject<T>(string key, T dataToStore)
        {
            try
            {
                using (var stream = new FileStream(_configDirPath + Path.DirectorySeparatorChar + key, FileMode.Create))
                {
                    var output = Newtonsoft.Json.JsonConvert.SerializeObject(dataToStore);
                    using (var sw = new StreamWriter(stream))
                    {
                        sw.Write(output);
                    }
                }
            }
            catch (Exception ex) { }
        }

        public static T RetrieveObject<T>(string key)
        {
            try
            {
                using (var stream = new FileStream(_configDirPath + Path.DirectorySeparatorChar + key, FileMode.Open))
                {
                    using (var sr = new StreamReader(stream))
                    {
                        var output = sr.ReadToEnd();

                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(output);
                    }
                }
            }
            catch
            {
                return default(T);
            }

        }
    }
}
