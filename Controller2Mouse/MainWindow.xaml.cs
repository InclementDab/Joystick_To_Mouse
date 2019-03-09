using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Controller2Mouse.Properties;
using SlimDX.DirectInput;

namespace Controller2Mouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private JoystickController _JoystickController { get; set; }
        private Joystick _ActiveJoystick { get; set; }

        [DllImport("kernel32")]
        static extern bool AllocConsole();


        public MainWindow() { }

        public MainWindow(JoystickController JoystickController)
        {
            // AllocConsole();

            InitializeComponent();
            _JoystickController = JoystickController;
            MainListView.ItemsSource = _JoystickController.ControllerList.Select(x => x);
        }

        private void Refresh_OnClick(object sender, RoutedEventArgs e)
        {
            MainListView.ItemsSource = _JoystickController.ControllerList.Select(x => x);
        }

        private void Select_OnClick(object sender, RoutedEventArgs e)
        {
            _ActiveJoystick = _JoystickController.AcquireJoystick(MainListView.SelectedItem as DeviceInstance);
            Log.Add("Selected " + _ActiveJoystick.Properties.ProductName);
        }

        private void MainWindow_OnLayoutUpdated(object sender, EventArgs e)
        {
            SelectButton.IsEnabled = MainListView.SelectedItems.Count == 1;
            Settings.Default.Save();
        }

        private void Start_OnClick(object sender, RoutedEventArgs e)
        {
            _ActiveJoystick.Properties.AxisMode =
                Settings.Default.absPos ? DeviceAxisMode.Absolute : DeviceAxisMode.Relative;

            Log.Add("Acquiring " + _ActiveJoystick.Properties.ProductName + "...");
            Log.Add(_ActiveJoystick.Acquire());
        }
    }
}
