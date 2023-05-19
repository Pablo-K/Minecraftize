using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Minecraftize
{
    class DataManager
    {
        private static readonly JsonSerializerSettings _options = new() { NullValueHandling = NullValueHandling.Ignore };
        public static void Write(object obj)
        {
            string fileName = "AverageColorsImages.json";
            var jsonString = JsonConvert.SerializeObject(obj, _options);
            File.WriteAllText(fileName, jsonString);
        }
        public static void WriteFileIfEmpty()
        {
            if (File.ReadAllText("AverageColorsImages.json").Length == 0)
            {
                var dir = Directory.GetFiles("Res/mc");
                int fileCount = dir.Length;
                List<List<string>> l = new List<List<string>>();
                for (int x = 0; x < fileCount; x++)
                {
                    l.Add(new List<string>());
                    l[x].Add(dir[x]);
                    Bitmap b = new Bitmap(dir[x].ToString());
                    Color c = ColorManager.GetAverageColor(b);
                    l[x].Add(c.R.ToString());
                    l[x].Add(c.G.ToString());
                    l[x].Add(c.B.ToString());
                }
                DataManager.Write(l);
            }
        }
    }
}
