using Hazdryx.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Minecraftize {
  public class IconsCacheManager {

    private Color[] _colors;
    private Image[] _icons;
    private readonly Dictionary<int, FastBitmap[]> _smallIconsCache;

    public Color[] Colors => _colors;

    public IconsCacheManager() {
      _colors = Array.Empty<Color>();
      _icons = Array.Empty<Image>();
      _smallIconsCache = new();
    }

    public void Initialize() {

      var colors = new List<Color>();
      var icons = new List<Image>();

      var text = File.ReadAllText("AverageColorsImages.json");
      var str = JsonSerializer.Deserialize<string[][]>(text)!;

      for (int i = 0; i < str.Length; i++) {
        int r = Convert.ToInt32(str[i][1]);
        int g = Convert.ToInt32(str[i][2]);
        int b = Convert.ToInt32(str[i][3]);
        colors.Add(Color.FromArgb(r, g, b));
        var path = str[i][0];
        var icon = Image.FromFile(path);
        icons.Add(icon);
      }

      _colors = colors.ToArray();
      _icons = icons.ToArray();
      _smallIconsCache.Add(4, new FastBitmap[_icons.Length]);
      _smallIconsCache.Add(8, new FastBitmap[_icons.Length]);
      _smallIconsCache.Add(12, new FastBitmap[_icons.Length]);

      for (int i = 0; i < _icons.Length; i++) {
        _smallIconsCache[4][i] = new FastBitmap(new Bitmap(_icons[i], 4, 4));
        _smallIconsCache[8][i] = new FastBitmap(new Bitmap(_icons[i], 8, 8));
        _smallIconsCache[12][i] = new FastBitmap(new Bitmap(_icons[i], 12, 12));
      }

    }

    public void DrawIconOnBitmap(FastBitmap source, int iconIndex) {

      if (source.Width <= 12) {
        var icon = _smallIconsCache[source.Width][iconIndex];
        lock (icon) { icon.CopyTo(source); }
      }
      else {
        var icon = _icons[iconIndex];
        lock (icon) {
          using var graphics = Graphics.FromImage(source.BaseBitmap);
          graphics.InterpolationMode = InterpolationMode.Default;
          Rectangle targetRect = new Rectangle(0, 0, source.BaseBitmap.Width, source.BaseBitmap.Height);
          graphics.DrawImage(icon, targetRect);
        }
      }

    }

  }
}
