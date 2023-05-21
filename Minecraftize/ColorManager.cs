using Hazdryx.Drawing;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Minecraftize {
  public class ColorManager {

    private readonly IconsCacheManager _iconsCacheManager;
    private readonly ConcurrentDictionary<Color, int> _closestColorsCache;

    public ColorManager() {
      _closestColorsCache = new();
      _iconsCacheManager = new IconsCacheManager();
      _iconsCacheManager.Initialize();
    }

    public void DrawIconOnBitmap(FastBitmap source) {
      var color = GetAverageColor(source);
      var iconIndex = FindClosestColorIndex(color);
      _iconsCacheManager.DrawIconOnBitmap(source, iconIndex);
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

      Color color = Color.FromArgb(redAverage, greenAverage, blueAverage);

      return color;

    }

    private int FindClosestColorIndex(Color targetColor) {

      if (_closestColorsCache.ContainsKey(targetColor)) return _closestColorsCache[targetColor];

      int closestColorIndex = 0;
      int closestDistance = CalculateColorDistance(targetColor, _iconsCacheManager.Colors[0]);

      for (int i = 0; i < _iconsCacheManager.Colors.Length; i++) {
        int distance = CalculateColorDistance(targetColor, _iconsCacheManager.Colors[i]);
        if (distance < closestDistance) {
          closestColorIndex = i;
          closestDistance = distance;
        }
      }

      _closestColorsCache.TryAdd(targetColor, closestColorIndex);

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
