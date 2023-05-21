using Hazdryx.Drawing;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace Minecraftize {
  public class Minecraftizer {

    private readonly ColorManager _colorManager;

    public Minecraftizer(ColorManager colorManager) {

      _colorManager = colorManager;

    }

    public Bitmap Minecraftize(Bitmap sourceBitmap, int squareSize) {

#if DEBUG
      var timer = System.Diagnostics.Stopwatch.StartNew();
#endif

      int horizontalSquaresCount = sourceBitmap.Width / squareSize;
      int verticalSquaresCount = sourceBitmap.Height / squareSize;

      var fastSourceBitmap = new FastBitmap(sourceBitmap);
      var finalImage = new FastBitmap(sourceBitmap.Width, sourceBitmap.Height);
      var squareBitmap = new FastBitmap(squareSize, squareSize);

      for (int i = 0; i < verticalSquaresCount * horizontalSquaresCount; i++) {

        int x = (i % horizontalSquaresCount) * squareSize;
        int y = (i / horizontalSquaresCount) * squareSize;

        fastSourceBitmap.CopyTo(squareBitmap, x, y, squareSize, squareSize);

        var avgBitmap = _colorManager.MinecraftizeBitmap(squareBitmap);

        var icon = new FastBitmap(avgBitmap);

        icon.CopyTo(finalImage, x, y, 0, 0, squareSize, squareSize);

      }

#if DEBUG
      timer.Stop();
      Debug.WriteLine($"Minecraftized image in {timer.Elapsed.TotalSeconds} seconds");
#endif

      return finalImage.BaseBitmap;

    }

  }
}
