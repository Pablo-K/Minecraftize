using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace Minecraftize
{
    public class MainWindowVM : ViewModel
    {
        private Bitmap _image;
        private Minecraftizer _minecraftizer;
        private bool _buttonEnabled;
        ImageSource _imagePanel;
        private int _value;
        private List<int> _sizes;
        public int Value { get { return _value; } set { _value = value; OnPropertyChanged(nameof(Value)); } }
        public List<int> Sizes {  get { return _sizes; } set { _sizes = value; OnPropertyChanged(nameof(Sizes)); } } 
        public ImageSource ImagePanel { get { return _imagePanel; } set { _imagePanel = value; OnPropertyChanged(nameof(ImagePanel)); } }
        public bool ButtonEnabled { get { return _buttonEnabled; } set { _buttonEnabled = value; OnPropertyChanged(nameof(ButtonEnabled)); } }

        public MainWindowVM()
        {
            DataManager.WriteFileIfEmpty();
            _minecraftizer = new Minecraftizer();
            ColorManager.CreateLists();
            Bitmap bm = new Bitmap("res/drop.png");
            _imagePanel = ImageSourceFromBitmap(bm);
            Sizes = new List<int> { 4,8,16,32 };
        }
        public void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            Value = (int)slider.Value;
        }

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        public void Image_FileDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length != 0)
            {
                FileInfo file = new FileInfo(files[0]);
                FileStream f1 = new FileStream(files[0], FileMode.Open,
                FileAccess.Read, FileShare.Read);
                byte[] BytesOfPic = new byte[Convert.ToInt32(file.Length)];
                f1.Read(BytesOfPic, 0, Convert.ToInt32(file.Length));

                if (file.Extension != ".png" && file.Extension != ".jpg" && file.Extension != ".jpeg") return;
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(BytesOfPic, 0, BytesOfPic.Length);
                    mStream.Seek(0, SeekOrigin.Begin);
                    _image = new Bitmap(mStream);
                    ButtonEnabled = true;
                    UpdateImage(_image);
                }
            }
        }
        private void UpdateImage(Bitmap bm)
        {
            ImagePanel = ImageSourceFromBitmap(bm);
        }

        public ICommand MinecraftizeClickCommand { get => new Command(MinecraftizeClick); }

        private async void MinecraftizeClick(object? obj)
        {
            Bitmap bm = await _minecraftizer.Minecraftize(_image, Value);
            ImagePanel = ImageSourceFromBitmap(bm);
        }
    }
}
