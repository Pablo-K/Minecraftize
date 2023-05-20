using System.Drawing;
using System.Windows;

namespace Minecraftize
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowVM _vm;
        public MainWindow()
        {
            _vm = new MainWindowVM();
            InitializeComponent();
            this.DataContext = _vm;
            this.SizeChanged += OnWindowSizeChanged;
        }
        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _vm.OnWindowSizeChanged(sender, e);
        }

        private void Image_FileDrop(object sender, DragEventArgs e)
        {
            _vm.Image_FileDrop(sender, e);
        }

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _vm.SliderValueChanged(sender, e);
        }

        private void SaveImageButton(object sender, RoutedEventArgs e)
        {
            _vm.SaveImageButton(sender, e);
        }
    }
}
