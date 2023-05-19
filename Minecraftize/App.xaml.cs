using System.Windows;

namespace Minecraftize
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindowVM mainWindowVM = new MainWindowVM();
            var MainWindow = new MainWindow();
            MainWindow.Show();
            
        }
    }
}
