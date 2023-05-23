using System;
using System.Drawing.Imaging;
using System.IO;

namespace Minecraftize
{
    public static class DialogsManager {


    public static string? ShowSaveImageDialog() {

      var dialog = new Microsoft.Win32.SaveFileDialog {
        FileName = "MinecraftizedImage",
        Filter = "png (*.png)|*.png",
        DefaultExt = "png",
      };

      if (dialog.ShowDialog() != true) return null;

      return dialog.FileName;

    }

    public static string? ShowChooseImageDialog() {

      var dlg = new Microsoft.Win32.OpenFileDialog {
        Filter = GetFileDialogImageFilter(),
        FilterIndex = 5
      };

      if (dlg.ShowDialog() != true) return null;

      string filename = dlg.FileName;
      FileInfo file = new FileInfo(filename);
      if (file.Extension != ".png" && file.Extension != ".jpg" && file.Extension != ".jpeg") return null;

      return filename;

    }
    public static string? ShowChooseVideoDialog() {

      var dlg = new Microsoft.Win32.OpenFileDialog {
        Filter = GetFileDialogVideoFilter(),
        FilterIndex = 5
      };

      if (dlg.ShowDialog() != true) return null;

      string filename = dlg.FileName;
      FileInfo file = new FileInfo(filename);
      if (file.Extension != ".mp4") return null;

      return filename;

    }

    private static string GetFileDialogImageFilter() {

      ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
      string sep = string.Empty;
      string filter = string.Empty;
      foreach (var c in codecs) {
        string codecName = c.CodecName!.Substring(8).Replace("Codec", "Files").Trim();
        filter = String.Format("{0}{1}{2} ({3})|{3}", filter, sep, codecName, c.FilenameExtension);
        sep = "|";
      }
      filter = String.Format("{0}{1}{2} ({3})|{3}", filter, sep, "All Files", "*.*");
      return filter;

    }
    private static string GetFileDialogVideoFilter() {

      string sep = string.Empty;
      string filter = string.Empty;
            filter = "MP4 files (*.mp4)|*.mp4|All files (*.*)|*.*";
      return filter;

    }

  }
}
