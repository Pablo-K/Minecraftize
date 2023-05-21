using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Minecraftize {
  public class MainWindowVM : ViewModel {

    private readonly Minecraftizer _minecraftizer;
    private Bitmap? _loadedImage = null;
    private ImageSource? _imageSource;
    private Bitmap? _minecraftizedImage = null;
    private int _sliderValue = 8;

    public int SliderValue { get { return _sliderValue; } set { _sliderValue = value; OnPropertyChanged(nameof(SliderValue)); } }
    public ImageSource ImageSource { get { return _imageSource!; } set { _imageSource = value; OnPropertyChanged(nameof(ImageSource)); } }

    public ICommand MinecraftizeClickCommand => new Command(MinecraftizeClick, (_) => _loadedImage != null);
    public ICommand SaveImageCommand => new Command(SaveImage, (_) => _minecraftizedImage != null);
    public ICommand ChooseImageCommand => new Command(ChooseImage);

    public MainWindowVM() {

      DataManager.WriteFileIfEmpty();

      _minecraftizer = new Minecraftizer(ColorManager.Initialize());

      UpdateImage(new Bitmap("res/drop.png"));

    }

    private void SaveImage(object? _) {
      string? filename = DialogsManager.ShowSaveImageDialog();
      if (filename is null) return;
      _minecraftizedImage!.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
    }

    public void Image_FileDrop(FileInfo file) {
      _loadedImage = (Bitmap)Bitmap.FromFile(file.FullName);
      UpdateImage(_loadedImage);
    }

    private void UpdateImage(Bitmap bm) {
      var handle = bm.GetHbitmap();
      var options = BitmapSizeOptions.FromEmptyOptions();
      var imgSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, options);
      this.ImageSource = imgSource;
    }

    private void MinecraftizeClick(object? _) {
      _minecraftizedImage = _minecraftizer.Minecraftize(_loadedImage!, _sliderValue);
      UpdateImage(_minecraftizedImage);
    }

    private void ChooseImage(object? _) {
      string? filename = DialogsManager.ShowChooseImageDialog();
      if (filename is null) return;
      _loadedImage = (Bitmap)Bitmap.FromFile(filename);
      UpdateImage(_loadedImage);
    }

  }
}
