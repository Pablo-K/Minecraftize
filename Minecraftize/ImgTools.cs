using FFMediaToolkit.Decoding;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Minecraftize
{
    public static class ImgTools
    {
        public static unsafe Bitmap ToBitmap(this ImageData imageData)
        {
            fixed (byte* p = imageData.Data)
            {
                return new Bitmap(imageData.ImageSize.Width, imageData.ImageSize.Height, imageData.Stride, PixelFormat.Format24bppRgb, new IntPtr(p));
            }
        }
        public static Bitmap GetNextFrame(MediaFile loadedVideo)
        {
            if (loadedVideo.Video.TryGetNextFrame(out var imageData))
            {
                return imageData.ToBitmap();
            }
            return new Bitmap(1, 1);
        }

        public static void AddFrame(VideoOutputStream video, Bitmap bm)
        {
            var rect = new Rectangle(Point.Empty, bm.Size);
            var bitLock = bm.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            var bitmapData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Bgr24, bm.Size);
            video.AddFrame(bitmapData);
            bm.UnlockBits(bitLock);
            bm.Dispose();
        }
        public static void GetBitmapsFromFile(MediaFile loadedVideo)
        {
            var file = loadedVideo;
            var i = 0;
            while (file.Video.TryGetNextFrame(out var imageData))
            {
                Bitmap bm = imageData.ToBitmap();
                Directory.CreateDirectory(Path.Join(Directory.GetCurrentDirectory(), "minecraftizedVideo"));

                bm.Save(Path.Join(Directory.GetCurrentDirectory(), "minecraftizedVideo/" + i + ".png"));
                bm.Dispose();
                i++;
            }
        }
    }
}
