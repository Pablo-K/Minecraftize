using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Minecraftize
{
    class Minecraftizer
    {
        public int Square { get; set; }
        public Minecraftizer() { }

        public async Task<Bitmap> Minecraftize(Bitmap bm, int size)
        {
            Bitmap bitmap = new Bitmap(bm.Width, bm.Height);
            Graphics g = Graphics.FromImage(bitmap);
            var dir = Directory.GetFiles("Res/mc");
            int fileCount = dir.Length;

            List<Color> averageColors = new List<Color>();
            int squaresWidth = bm.Width / size;
            int squaresHeight = bm.Height / size;

            Task thread1 = Task.Run(() =>
        {
            for (int x = 0; x < squaresHeight; x++)
            {
                for (int i = 0; i < squaresWidth; i++)
                {
                    Bitmap b = new Bitmap(size, size);
                    for (int j = 0; j < size; j++)
                    {
                        for (int k = 0; k < size; k++)
                        {
                            b.SetPixel(j, k, bm.GetPixel(i * size + j, x * size + k));
                        }
                    }
                    lock (averageColors)
                    {
                        averageColors.Add(ColorManager.GetAverageColor(b));
                    }
                }
            }
        });
            Bitmap finalImage = new Bitmap(squaresWidth * size, squaresHeight * size);
            Task thread2 = Task.Run(() =>
            {
                int counter = 0;
                for (int x = 0; x < squaresHeight; x++)
                {
                    for (int i = 0; i < squaresWidth; i++)
                    {
                        while (averageColors.Count <= counter) { }
                        Bitmap ico = new Bitmap(ColorManager.Icons[ColorManager.FindClosestColorIndex(averageColors[counter])]);
                        Bitmap icon = new Bitmap(ico, new Size(size, size));
                        for (int j = 0; j < size; j++)
                        {
                            for (int k = 0; k < size; k++)
                            {
                                finalImage.SetPixel(i * size + j, x * size + k, icon.GetPixel(j, k));
                            }
                        }
                        counter++;
                    }
                }
            });
            Task.WaitAll(thread1, thread2);
            return finalImage;
        }
    }
}
