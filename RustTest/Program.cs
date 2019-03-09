using System;
using System.Runtime.InteropServices;
using static Joy2Mouse.Win32;

namespace Joy2Mouse
{
    class Program
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            AllocConsole();
#endif
            MainWindow Window = new MainWindow
            {
                Content = new MainControl()
            };
            Window.Show();
        }
    }
}
