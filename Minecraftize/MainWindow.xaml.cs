using System.Drawing;
using System.IO;
using System.Windows;

namespace Minecraftize {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {

    public MainWindow() {

      InitializeComponent();

    }

    private void Image_FileDrop(object sender, DragEventArgs e) {

      string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

      if (files.Length != 0) return;

      var file = new FileInfo(files[0]);

      if (file.Extension != ".png" && file.Extension != ".jpg" && file.Extension != ".jpeg") return;

      ((MainWindowVM)this.DataContext).Image_FileDrop(file);

    }

  }
}
