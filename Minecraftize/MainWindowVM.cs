using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        private bool _saveButtonEnabled;
        string _dir;
        Bitmap _minecraftizedImage;
        ImageSource _imagePanel;
        private int _value;
        private int _imageWidth;
        private int _imageHeight;
        private List<int> _sizes;
        public int Value { get { return _value; } set { _value = value; OnPropertyChanged(nameof(Value)); } }
        public int ImageWidth { get { return _imageWidth; } set { _imageWidth = value; OnPropertyChanged(nameof(ImageWidth)); } }
        public int ImageHeight { get { return _imageHeight; } set { _imageHeight = value; OnPropertyChanged(nameof(ImageHeight)); } }
        public List<int> Sizes { get { return _sizes; } set { _sizes = value; OnPropertyChanged(nameof(Sizes)); } }
        public ImageSource ImagePanel { get { return _imagePanel; } set { _imagePanel = value; OnPropertyChanged(nameof(ImagePanel)); } }
        public bool ButtonEnabled { get { return _buttonEnabled; } set { _buttonEnabled = value; OnPropertyChanged(nameof(ButtonEnabled)); } }
        public bool SaveButtonEnabled { get { return _saveButtonEnabled; } set { _saveButtonEnabled = value; OnPropertyChanged(nameof(SaveButtonEnabled)); } }

        public MainWindowVM()
        {
            DataManager.WriteFileIfEmpty();
            _minecraftizer = new Minecraftizer();
            ColorManager.CreateLists();
            Bitmap bm = new Bitmap("res/drop.png");
            ImageWidth = 600;
            ImageHeight = 300;
            _imagePanel = ImageSourceFromBitmap(bm);
            Sizes = new List<int> { 4, 8, 16, 32 };
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
        public void SaveImageButton(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "MinecraftizedImage";
            dlg.Filter = "";

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;
            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                dlg.Filter = String.Format("{0}{1}{2} ({3})|{3}", dlg.Filter, sep, codecName, c.FilenameExtension);
                sep = "|";
            }

            dlg.Filter = String.Format("{0}{1}{2} ({3})|{3}", dlg.Filter, sep, "All Files", "*.*");
            dlg.InitialDirectory = _dir;
            dlg.DefaultExt = ".png";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                _minecraftizedImage.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
            }
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
                    _dir = file.Directory.ToString();
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
            _minecraftizedImage = await _minecraftizer.Minecraftize(_image, Value);
            SaveButtonEnabled = true;
            ImagePanel = ImageSourceFromBitmap(_minecraftizedImage);
        }

        internal void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newWindowHeight = e.NewSize.Height;
            double newWindowWidth = e.NewSize.Width;
            double prevWindowHeight = e.PreviousSize.Height;
            double prevWindowWidth = e.PreviousSize.Width;
            ImageWidth = (int)(newWindowWidth * 0.8);
            ImageHeight = (int)(newWindowHeight * 1);
        }

        internal void ChooseImageButton(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "";
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;
            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                dlg.Filter = String.Format("{0}{1}{2} ({3})|{3}", dlg.Filter, sep, codecName, c.FilenameExtension);
                sep = "|";
            }

            dlg.Filter = String.Format("{0}{1}{2} ({3})|{3}", dlg.Filter, sep, "All Files", "*.*");
            dlg.InitialDirectory = _dir;
            dlg.DefaultExt = "*.*";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                FileInfo file = new FileInfo(filename);
                FileStream f1 = new FileStream(filename, FileMode.Open,
                FileAccess.Read, FileShare.Read);;
                byte[] BytesOfPic = new byte[Convert.ToInt32(file.Length)];
                f1.Read(BytesOfPic, 0, Convert.ToInt32(file.Length));

                if (file.Extension != ".png" && file.Extension != ".jpg" && file.Extension != ".jpeg") return;
                using (MemoryStream mStream = new MemoryStream())
                {
                    _dir = file.Directory.ToString();
                    mStream.Write(BytesOfPic, 0, BytesOfPic.Length);
                    mStream.Seek(0, SeekOrigin.Begin);
                    _image = new Bitmap(mStream);
                    ButtonEnabled = true;
                    UpdateImage(_image);
                }
            }

        }
    }
}
