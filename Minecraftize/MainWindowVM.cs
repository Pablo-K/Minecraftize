using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Documents;
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
    private bool _isMinecraftizingInProgress;

    public int SliderValue { get { return _sliderValue; } set { _sliderValue = value; OnPropertyChanged(); } }
    public ImageSource ImageSource { get { return _imageSource!; } set { _imageSource = value; OnPropertyChanged(); } }
    public bool IsMinecraftizingInProgress { get => _isMinecraftizingInProgress; set { _isMinecraftizingInProgress = value; OnPropertyChanged(); } }

    public ICommand MinecraftizeClickCommand { get; }
    public ICommand SaveImageCommand { get; }
    public ICommand ChooseImageCommand { get; }

    public MainWindowVM() {

      DataManager.WriteFileIfEmpty();

      _minecraftizer = new Minecraftizer();

      UpdateImage(new Bitmap("res/drop.png"));

      _isMinecraftizingInProgress = false;

      this.MinecraftizeClickCommand = new Command(
        execute: MinecraftizeClick,
        canExecute: (_) => _loadedImage != null && !this.IsMinecraftizingInProgress); 

      this.SaveImageCommand = new Command(
        execute: SaveImage,
        canExecute: (_) => _minecraftizedImage != null && !this.IsMinecraftizingInProgress); 

      this.ChooseImageCommand = new Command(
        execute: ChooseImage,
        canExecute: (_) => !this.IsMinecraftizingInProgress);

    }

    private void SaveImage(object? _) {
      string? filename = DialogsManager.ShowSaveImageDialog();
      if (filename is null) return;
      _minecraftizedImage!.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
    }

    public void Image_FileDrop(FileInfo file) {
      _loadedImage?.Dispose();
      _loadedImage = (Bitmap)Bitmap.FromFile(file.FullName);
      UpdateImage(_loadedImage);
      RaiseCanExecuteChanged();
    }

    private void UpdateImage(Bitmap bm) {
      var handle = bm.GetHbitmap();
      var options = BitmapSizeOptions.FromEmptyOptions();
      var imgSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, options);
      this.ImageSource = imgSource;
    }

    private async void MinecraftizeClick(object? _) {
      this.IsMinecraftizingInProgress = true;
      var minecraftizedImage = await _minecraftizer.Minecraftize(_loadedImage!, _sliderValue);
      this.IsMinecraftizingInProgress = false;
      _minecraftizedImage?.Dispose();
      _minecraftizedImage = minecraftizedImage;
      UpdateImage(_minecraftizedImage);
      RaiseCanExecuteChanged();
    }

    private void ChooseImage(object? _) {
      string? filename = DialogsManager.ShowChooseImageDialog();
      if (filename is null) return;
      _loadedImage?.Dispose();
      _loadedImage = (Bitmap)Bitmap.FromFile(filename);
      UpdateImage(_loadedImage);
      RaiseCanExecuteChanged();
    }

    private void RaiseCanExecuteChanged() {
      ((Command)this.MinecraftizeClickCommand).RaiseCanExecuteChanged();
      ((Command)this.ChooseImageCommand).RaiseCanExecuteChanged();
      ((Command)this.SaveImageCommand).RaiseCanExecuteChanged();
    }

  }
}
