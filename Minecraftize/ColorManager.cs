using Hazdryx.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Minecraftize {
  public class ColorManager {

    private readonly Color[] _colors;
    private readonly Image[] _icons;
    private readonly Dictionary<Color, int> _closestColorsCache;

    private ColorManager(IEnumerable<Color> colors, IEnumerable<Image> icons) {
      _colors = colors.ToArray();
      _icons = icons.ToArray();
      _closestColorsCache = new();
    }

    public static ColorManager Initialize() {

      var colors = new List<Color>();
      var icons = new List<Image>();

      string[][] str = JsonSerializer.Deserialize<string[][]>(File.ReadAllText("AverageColorsImages.json"))!;

      for (int i = 0; i < str.Length; i++) {
        int r = Convert.ToInt32(str[i][1]);
        int g = Convert.ToInt32(str[i][2]);
        int b = Convert.ToInt32(str[i][3]);
        colors.Add(Color.FromArgb(r, g, b));
        var path = str[i][0];
        icons.Add(Image.FromFile(path));
      }

      return new ColorManager(colors, icons);

    }

    public Bitmap MinecraftizeBitmap(FastBitmap source) {
      var color = GetAverageColor(source);
      var iconIndex = FindClosestColorIndex(color);
      var bitmap = GetBitmapByIndex(iconIndex, source.Width, source.Height);
      return bitmap;
    }

    public static Color GetAverageColor(FastBitmap bm) {

      int width = bm.Width;
      int height = bm.Height;

      long redSum = 0;
      long greenSum = 0;
      long blueSum = 0;

      var buffer = new int[width * height];

      bm.Read(buffer);

      foreach (var argb in buffer) {
        redSum += (argb & 0x00FF0000L) >> 16;
        greenSum += (argb & 0x0000FF00L) >> 8;
        blueSum += (argb & 0x000000FFL);
      }

      int totalPixels = width * height;

      byte redAverage = (byte)(redSum / totalPixels);
      byte greenAverage = (byte)(greenSum / totalPixels);
      byte blueAverage = (byte)(blueSum / totalPixels);

      Color c = Color.FromArgb(redAverage, greenAverage, blueAverage);
      return c;

    }

    private Bitmap GetBitmapByIndex(int index, int width, int height) {
      var img = _icons[index];
      var bitmap = new Bitmap(img, new Size(width, height));
      return bitmap;
    }

    private int FindClosestColorIndex(Color targetColor) {

      if (_closestColorsCache.ContainsKey(targetColor)) return _closestColorsCache[targetColor];

      int closestColorIndex = 0;
      int closestDistance = CalculateColorDistance(targetColor, _colors[0]);

      for (int i = 0; i < _colors.Length; i++) {
        int distance = CalculateColorDistance(targetColor, _colors[i]);
        if (distance < closestDistance) {
          closestColorIndex = i;
          closestDistance = distance;
        }
      }

      _closestColorsCache.Add(targetColor, closestColorIndex);

      return closestColorIndex;

    }

    private int CalculateColorDistance(Color color1, Color color2) {
      int rDiff = color1.R - color2.R;
      int gDiff = color1.G - color2.G;
      int bDiff = color1.B - color2.B;
      return (rDiff * rDiff) + (gDiff * gDiff) + (bDiff * bDiff);
    }

  }
}
