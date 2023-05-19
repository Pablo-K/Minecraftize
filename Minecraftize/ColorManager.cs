using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace Minecraftize
{
    class ColorManager
    {

        private static List<Color> _colors;
        public static List<string> Icons;
        public static void CreateLists()
        {
            _colors = new List<Color>();
            Icons = new List<string>();
            string s = File.ReadAllText("AverageColorsImages.json");
            string[][] str = JsonSerializer.Deserialize<string[][]>(File.ReadAllText("AverageColorsImages.json"));
            for (int i = 0; i < str.Length; i++)
            {
                Icons.Add(str[i][0]);
                _colors.Add(Color.FromArgb(Convert.ToInt32(str[i][1]),Convert.ToInt32(str[i][2]), Convert.ToInt32(str[i][3])));
            }
        }
        public static int FindClosestColorIndex(Color targetColor)
        {
            Color closestColor = _colors[0];
            int closestDistance = CalculateColorDistance(targetColor, closestColor);

            foreach (Color color in _colors)
            {
                int distance = CalculateColorDistance(targetColor, color);
                if (distance < closestDistance)
                {
                    closestColor = color;
                    closestDistance = distance;
                }
            }
            return _colors.IndexOf(closestColor);
        }
        public static int CalculateColorDistance(Color color1, Color color2)
        {
            int rDiff = color1.R - color2.R;
            int gDiff = color1.G - color2.G;
            int bDiff = color1.B - color2.B;

            return (rDiff * rDiff) + (gDiff * gDiff) + (bDiff * bDiff);
        }
        public static Color GetAverageColor(Bitmap bm)
        {
            int width = bm.Width;
            int height = bm.Height;

            int redSum = 0;
            int greenSum = 0;
            int blueSum = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixelColor = bm.GetPixel(x, y);

                    redSum += pixelColor.R;
                    greenSum += pixelColor.G;
                    blueSum += pixelColor.B;
                }
            }

            int totalPixels = width * height;
            int averageRed = redSum / totalPixels;
            int averageGreen = greenSum / totalPixels;
            int averageBlue = blueSum / totalPixels;

            Color c = Color.FromArgb(averageRed, averageGreen, averageBlue);
            return c;

        }
    }
}
