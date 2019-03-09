using System;
using System.Windows;
using System.Windows.Controls;

namespace Joy2Mouse
{
    /// <summary>
    /// Interaction logic for MainWIndow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(int x = 800, int y = 450)
        {
            InitializeComponent();

            Width = x;
            Height = y;
        }
    }
}
