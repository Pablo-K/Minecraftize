using Hazdryx.Drawing;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Minecraftize {
  public class Minecraftizer {

    private readonly ColorManager _colorManager;

    private int _squareSize;
    private int _horizontalSquaresCount;
    private int _verticalSquaresCount;
    private FastBitmap _fastSourceBitmap;
    private FastBitmap _finalImage;

    private FastBitmap _squareBitmap;

    public Minecraftizer() {

      _colorManager = new ColorManager();

    }

    public async Task<Bitmap> Minecraftize(Bitmap sourceBitmap, int squareSize) {

#if DEBUG
      var timer = System.Diagnostics.Stopwatch.StartNew();
#endif

      _squareSize = squareSize;
      _horizontalSquaresCount = sourceBitmap.Width / squareSize;
      _verticalSquaresCount = sourceBitmap.Height / squareSize;

      _squareBitmap = new FastBitmap(squareSize, squareSize);
      _fastSourceBitmap = new FastBitmap(sourceBitmap);
      _finalImage = new FastBitmap(sourceBitmap.Width, sourceBitmap.Height);

      await Parallel.ForEachAsync(
        Partitioner.Create(0, _verticalSquaresCount * _horizontalSquaresCount, 1_000_000).GetDynamicPartitions(),
        new ParallelOptions { MaxDegreeOfParallelism = 2 },
        MinecraftizeAsync
        );

#if DEBUG
      timer.Stop();
      Debug.WriteLine($"Minecraftized image in {timer.Elapsed.TotalSeconds} seconds");
#endif

      _squareBitmap.Dispose();
      _fastSourceBitmap.Dispose();

      return _finalImage.BaseBitmap;

    }

    private ValueTask MinecraftizeAsync(Tuple<int, int> range, CancellationToken ct) {

      for (int i = range.Item1; i < range.Item2; i++) {

        int x = (i % _horizontalSquaresCount) * _squareSize;
        int y = (i / _horizontalSquaresCount) * _squareSize;

        _fastSourceBitmap.CopyTo(_squareBitmap, x, y, _squareSize, _squareSize);

        _colorManager.DrawIconOnBitmap(_squareBitmap);

        WriteIconOnFinalImage(_squareBitmap.BaseBitmap, x, y);

      }

      return ValueTask.CompletedTask;

    }

    private async void WriteIconOnFinalImage(Bitmap icon, int x, int y) {

      lock (icon) {

        BitmapData bmpData = icon.LockBits(new Rectangle(0, 0, icon.Width, icon.Height),
                                           ImageLockMode.ReadOnly,
                                           icon.PixelFormat);

        IntPtr ptr = bmpData.Scan0;
        int bytesPerPixel = Image.GetPixelFormatSize(icon.PixelFormat) / 8;
        int stride = bmpData.Stride;
        int width = icon.Width;
        int height = icon.Height;

        unsafe {
          for (int iconY = 0; iconY < height; iconY++) {
            byte* row = (byte*)ptr + (iconY * stride);
            for (int iconX = 0; iconX < width; iconX++) {
              byte* pixel = row + (iconX * bytesPerPixel);
              byte blue = pixel[0];
              byte green = pixel[1];
              byte red = pixel[2];
              var color = Color.FromArgb(red, green, blue);
              _finalImage.Set(x + iconX, y + iconY, color);
            }
          }
        }

        icon.UnlockBits(bmpData);

      }

    }

  }
}
