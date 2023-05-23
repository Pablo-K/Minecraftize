using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Minecraftize
{
    public class MainWindowVM : ViewModel
    {

        private readonly Minecraftizer _minecraftizer;
        private Bitmap? _loadedImage = null;
        private MediaFile? _loadedVideo = null;
        private MediaFile? _minecraftizedVideo = null;
        private ImageSource? _imageSource;
        private Bitmap? _minecraftizedImage = null;
        private int _sliderValue = 8;
        private bool _isMinecraftizingInProgress;
        private string _filePath;

        public int SliderValue { get { return _sliderValue; } set { _sliderValue = value; OnPropertyChanged(); } }
        public ImageSource ImageSource { get { return _imageSource!; } set { _imageSource = value; OnPropertyChanged(); } }
        public bool IsMinecraftizingInProgress { get => _isMinecraftizingInProgress; set { _isMinecraftizingInProgress = value; OnPropertyChanged(); } }

        public ICommand MinecraftizeClickCommand { get; }
        public ICommand SaveImageCommand { get; }
        public ICommand ChooseImageCommand { get; }
        public ICommand ChooseVideoCommand { get; }

        public MainWindowVM()
        {

            DataManager.WriteFileIfEmpty();

            _minecraftizer = new Minecraftizer();

            UpdateImage(new Bitmap("res/drop.png"));

            _isMinecraftizingInProgress = false;

            this.MinecraftizeClickCommand = new Command(
              execute: MinecraftizeClick,
              canExecute: (_) => (_loadedImage != null || _loadedVideo != null) && !this.IsMinecraftizingInProgress);

            this.SaveImageCommand = new Command(
              execute: SaveImage,
              canExecute: (_) => _minecraftizedImage != null && !this.IsMinecraftizingInProgress);

            this.ChooseImageCommand = new Command(
              execute: ChooseImage,
              canExecute: (_) => !this.IsMinecraftizingInProgress);

            this.ChooseVideoCommand = new Command(
              execute: ChooseVideo,
              canExecute: (_) => !this.IsMinecraftizingInProgress);

        }

        private void SaveImage(object? _)
        {
            string? filename = DialogsManager.ShowSaveImageDialog();
            if (filename is null) return;
            _minecraftizedImage!.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
        }

        public void Image_FileDrop(FileInfo file)
        {
            _loadedImage?.Dispose();
            _loadedImage = (Bitmap)Bitmap.FromFile(file.FullName);
            UpdateImage(_loadedImage);
            RaiseCanExecuteChanged();
        }

        private void UpdateImage(Bitmap bm)
        {
            var handle = bm.GetHbitmap();
            var options = BitmapSizeOptions.FromEmptyOptions();
            var imgSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, options);
            this.ImageSource = imgSource;
        }

        private async void MinecraftizeClick(object? _)
        {
            this.IsMinecraftizingInProgress = true;
            if (_loadedImage != null)
            {
                var minecraftizedImage = await _minecraftizer.Minecraftize(_loadedImage!, _sliderValue);
                this.IsMinecraftizingInProgress = false;
                _minecraftizedImage?.Dispose();
                _minecraftizedImage = minecraftizedImage;
            }
            if (_loadedVideo != null)
            {
                List<Bitmap> minecraftizedBitmaps = new List<Bitmap>();
                ImgTools.GetBitmapsFromFile(_loadedVideo);
                var dir = Directory.GetFiles(Path.Join(Directory.GetCurrentDirectory(), "minecraftizedVideo"));
                dir = dir.OrderBy(x => int.Parse(x.Split("\\").Last().Replace(".png", ""))).ToArray();
                int fileCount = dir.Length;
                for (int j = 0; j < fileCount; j++)
                {
                    Bitmap bm = new Bitmap(dir[j]);
                    minecraftizedBitmaps.Add(await _minecraftizer.Minecraftize(bm, _sliderValue));
                }
                var settings = new VideoEncoderSettings(width: minecraftizedBitmaps.FirstOrDefault().Width, height: minecraftizedBitmaps.FirstOrDefault().Height, framerate: 30, codec: VideoCodec.H264);
                settings.EncoderPreset = EncoderPreset.Fast;
                settings.CRF = 17;
                using (var file = MediaBuilder.CreateContainer(_filePath.Remove(_filePath.Length - 4) + "i.mp4").WithVideo(settings).Create())
                {
                    for (int j = 0; j < minecraftizedBitmaps.Count; j++)
                    {
                        ImgTools.AddFrame(file.Video, minecraftizedBitmaps[j]);
                    }
                }
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    Directory.Delete(Path.Join(Directory.GetCurrentDirectory(), "minecraftizedVideo"), true);
                });
            }
            IsMinecraftizingInProgress = false;
            RaiseCanExecuteChanged();
        }

        private void ChooseImage(object? _)
        {
            string? filename = DialogsManager.ShowChooseImageDialog();
            if (filename is null) return;
            _filePath = filename;
            _loadedImage?.Dispose();
            _loadedImage = (Bitmap)Bitmap.FromFile(filename);
            UpdateImage(_loadedImage);
            RaiseCanExecuteChanged();
        }
        private void ChooseVideo(object? _)
        {
            FFmpegLoader.FFmpegPath = FFmpegLoader.FFmpegPath + "ffmpeg/x86_64";
            string? filename = DialogsManager.ShowChooseVideoDialog();
            if (filename is null) return;
            _filePath = filename;
            _loadedVideo?.Dispose();
            _loadedVideo = MediaFile.Open(@filename);
            _loadedVideo.Video.TryGetNextFrame(out var imageData);
            var image = imageData.ToBitmap();
            UpdateImage(image);
            RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged()
        {
            ((Command)this.MinecraftizeClickCommand).RaiseCanExecuteChanged();
            ((Command)this.ChooseImageCommand).RaiseCanExecuteChanged();
            ((Command)this.SaveImageCommand).RaiseCanExecuteChanged();
        }

    }
}
