using System.Windows;

namespace Controller2Mouse
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public JoystickController MainController = new JoystickController();

        public App()
        {
            MainWindow Window = new MainWindow(MainController);
            Window.Show();
        }
    }
}
