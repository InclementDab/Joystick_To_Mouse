using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Joy2Mouse.Win32;

namespace OBS_Mouse_Follower
{
    class Program
    {
        private const uint WM_KEYDOWN = 0x100;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("User32.dll")]
        public static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        [STAThread]
        static void Main(string[] args)
        {

            Process[] ProcessList = Process.GetProcessesByName("obs64");

            Process OBS = ProcessList[0];
            Console.WriteLine(OBS.MainWindowTitle);

            INPUT[] RightInput = new INPUT[1];
            RightInput[0].type = 1;
            RightInput[0].U.ki.wVk = VirtualKeyShort.F5;
            RightInput[0].U.ki.dwFlags = KEYEVENTF.EXTENDEDKEY;

            INPUT[] LeftInput = new INPUT[1];
            LeftInput[0].type = 1;
            LeftInput[0].U.ki.wVk = VirtualKeyShort.F6;
            LeftInput[0].U.ki.dwFlags = KEYEVENTF.EXTENDEDKEY;
            SendInput(1, LeftInput, INPUT.Size);


            bool state = false;
            while (true)
            {
                Thread.Sleep(100);
                state = Cursor.Position.X > 0;
                if (state)
                {
                    Console.WriteLine(SendInput(1, LeftInput, INPUT.Size));
                }
                if (!state)
                {
                    Console.WriteLine(SendInput(1, RightInput, INPUT.Size));
                }

            }

        }
    }
}
