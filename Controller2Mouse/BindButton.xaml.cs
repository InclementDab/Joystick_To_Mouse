using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SlimDX;
using SlimDX.DirectInput;
using Key = System.Windows.Input.Key;

namespace Controller2Mouse
{
    /// <summary>
    /// Interaction logic for BindButton.xaml
    /// </summary>
    public partial class BindButton : Window
    {
        private Joystick _ActiveJoystick;
        public BindButton(double Xpos, double Ypos, Joystick _activeJoystick)
        {
            InitializeComponent();
            Left = Xpos - Width/2;
            Top = Ypos - Height/2;
            _ActiveJoystick = _activeJoystick;
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }

        
    }
}
