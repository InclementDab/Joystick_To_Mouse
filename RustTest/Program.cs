using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SlimDX.DirectInput;
using static RustTest.Win32;
using RustTest.Properties;

namespace RustTest
{
    class Program
    {

        const double sensitivity = 0.5;
        const int refreshRate = 60;

        public static void Main(string[] args)
        {
            List<DeviceInstance> directInputList = new List<DeviceInstance>();
            DirectInput directInput = new DirectInput();

            directInputList.AddRange(directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly));


            if (directInputList.Count == 0)
            {
                Console.WriteLine("No Devices Found!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Devices Found");

            int i = 0;

            foreach (DeviceInstance device in directInputList)
            {
                Console.WriteLine(i + ": " + device.InstanceName);
                i++;
            }

            string input = Console.ReadLine();

            if (!int.TryParse(input, out i))
            {
                switch (input)
                {
                    case ("options"):
                        HandleSettings();
                        Settings.Default.Save();
                        break;

                    default:
                        break;
                }
            }






            Joystick ActiveJoystick = new Joystick(directInput, directInputList[i].InstanceGuid);
            foreach (DeviceObjectInstance doi in ActiveJoystick.GetObjects(ObjectDeviceType.Axis))
            {
                ActiveJoystick.GetObjectPropertiesById((int)doi.ObjectType).SetRange(-5000, 5000);
            }
            ActiveJoystick.Properties.AxisMode = DeviceAxisMode.Absolute;
            ActiveJoystick.Acquire();

            JoystickState ActiveJoystickState = ActiveJoystick.GetCurrentState();

            int screenWidth = InternalGetSystemMetrics(0);
            int screenHeight = InternalGetSystemMetrics(1);

            int to_x = screenWidth / 2;
            int to_y = screenHeight / 2;

            int mic_x = (int)Math.Round(to_x * 65536.0 / screenWidth);
            int mic_y = (int)Math.Round(to_y * 65536.0 / screenHeight);

            int[] defaultDeviceState =
            {
                ActiveJoystickState.X,
                ActiveJoystickState.Y,
                ActiveJoystickState.RotationZ
            };

            INPUT[] MouseInputs = new INPUT[1];

            string keyDown = null;

            while (true)
            {
            loop:

                Thread.Sleep(100 / refreshRate);
                ActiveJoystickState = ActiveJoystick.GetCurrentState();

                Console.WriteLine(keyDown);
                if (ActiveJoystick.Poll().IsFailure || ActiveJoystick.GetCurrentState(ref ActiveJoystickState).IsFailure)
                {
                    Console.WriteLine("Polling Failed");
                    goto loop;
                }

                double factor = .01;



                MouseInputs[0].U.mi.dx = (int)((ActiveJoystickState.X - defaultDeviceState[0]) * sensitivity * factor) * (Settings.Default.InvertX ? -1 : 1);
                MouseInputs[0].U.mi.dy = (int)((ActiveJoystickState.Y - defaultDeviceState[1]) * sensitivity * factor) * (Settings.Default.InvertX ? -1 : 1);
                MouseInputs[0].type = 0;
                MouseInputs[0].U.mi.dwFlags = MOUSEEVENTF.MOVE;
                SendInput(1, MouseInputs, INPUT.Size);



                KeyInputs[0].type = 1;


                if ((ActiveJoystick.GetCurrentState().RotationZ < 100 && ActiveJoystick.GetCurrentState().RotationZ > -100) && keyDown != null)// (KeyInputs[0].U.ki.dwFlags == KEYEVENTF.SCANCODE)
                {

                    KeyInputs[0].U.ki.dwFlags = KEYEVENTF.KEYUP;
                    KeyInputs[0].U.ki.wScan = 0;
                    KeyInputs[0].U.ki.
                    SendInput(1, KeyInputs, INPUT.Size);
                    keyDown = null;
                    goto loop;
                }


                if (KeyInputs[0].U.ki.dwFlags == KEYEVENTF.SCANCODE && keyDown != null) // oh look, you're holding the key!
                    goto loop;

                if ((ActiveJoystick.GetCurrentState().RotationZ > defaultDeviceState[2] + 100) && keyDown != "D") // (KeyInputs[0].U.ki.wScan != ScanCodeShort.KEY_D)
                {
                    KeyInputs[0].U.ki.dwFlags = KEYEVENTF.SCANCODE;
                    KeyInputs[0].U.ki.wScan = ScanCodeShort.KEY_D;
                    keyDown = "D";
                }

                if ((ActiveJoystick.GetCurrentState().RotationZ < defaultDeviceState[2] - 100) && keyDown != "A") // (KeyInputs[0].U.ki.wScan != ScanCodeShort.KEY_A)
                {
                    KeyInputs[0].U.ki.dwFlags = KEYEVENTF.SCANCODE;
                    KeyInputs[0].U.ki.wScan = ScanCodeShort.KEY_A;
                    keyDown = "A";
                }

                SendInput(1, KeyInputs, INPUT.Size);
            }
        }

        private static void HandleSettings()
        {
        switchStart:
            Console.WriteLine("Invert X Axis: " + Settings.Default.InvertX);
            Console.WriteLine("Invert Y Axis: " + Settings.Default.InvertY);

            switch (Console.ReadLine().ToLower())
            {
                case ("invertx"):
                    Settings.Default.InvertX = Convert.ToBoolean(Console.ReadLine());
                    goto switchStart;
                case ("inverty"):
                    Settings.Default.InvertY = Convert.ToBoolean(Console.ReadLine());
                    goto switchStart;
                default:
                    return;
            }
        }

        private static void SendKey(VirtualKeyShort keyShort)
        {
            INPUT KeyInput = new INPUT
            {
                type = 1
            };
            KeyInput.U.ki = new KEYBDINPUT
            {
                Vk = (ushort)keyShort,
                Scan = 0,
                Flags = 2,
                Time = 0,
                ExtraInfo = IntPtr.Zero
            };

            INPUT[] inputs = new INPUT[] {KeyInput};
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
    }


}
