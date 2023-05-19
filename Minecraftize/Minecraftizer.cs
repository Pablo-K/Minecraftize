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
        public Minecraftizer()
        {

        }

        public async Task<Bitmap> Minecraftize(Bitmap bm, int size)
        {
            Bitmap bitmap = new Bitmap(bm.Width, bm.Height);
            Graphics g = Graphics.FromImage(bitmap);
            var dir = Directory.GetFiles("Res/mc");
            int fileCount = dir.Length;

            List<Color> averageColors = new List<Color>();
            int squresWidth = bm.Width / size;
            int squaresHeight = bm.Height / size;

            List<Task> tasks = new List<Task>();

            for (int x = 0; x < squaresHeight; x++)
            {
                {
                    for (int i = 0; i < squresWidth; i++)
                    {
                        Bitmap b = new Bitmap(size, size);
                        for (int j = 0; j < size; j++)
                        {
                            for (int k = 0; k < size; k++)
                            {
                                b.SetPixel(j, k, bm.GetPixel(i * size + j, x * size + k));
                            }
                        }
                        averageColors.Add(ColorManager.GetAverageColor(b));
                    }
                };
            }
            Bitmap obraz = new Bitmap(squresWidth * size, squaresHeight * size);
            int licznik = 0;
            for (int x = 0; x < squaresHeight; x++)
            {
                for (int i = 0; i < squresWidth; i++)
                {
                    Bitmap ico = new Bitmap(ColorManager.Icons[ColorManager.FindClosestColorIndex(averageColors[licznik])]);
                    Bitmap icon = new Bitmap(ico, new Size(size,size)); 
                    for (int j = 0; j < size; j++)
                    {
                        for (int k = 0; k < size; k++)
                        {
                            obraz.SetPixel(i * size + j, x * size + k, icon.GetPixel(j,k));
                        }
                    }
                    licznik++;
                }
            }



            return obraz;
        }
    }
}
