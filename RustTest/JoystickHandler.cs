using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Joy2Mouse.Properties;
using SlimDX.DirectInput;
using static Joy2Mouse.Win32;

namespace Joy2Mouse
{
    class JoystickHandler
    {

        const double sensitivity = 0.5;
        const int refreshRate = 60;
        const double factor = .01;


        public JoystickHandler()
        {
            List<DeviceInstance> directInputList = new List<DeviceInstance>();
            DirectInput directInput = new DirectInput();

            directInputList.AddRange(directInput.GetDevices(DeviceClass.GameController,
                DeviceEnumerationFlags.AttachedOnly));


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
            // Startup Complete


            Joystick ActiveJoystick = new Joystick(directInput, directInputList[i].InstanceGuid);
            foreach (DeviceObjectInstance doi in ActiveJoystick.GetObjects(ObjectDeviceType.Axis))
            {
                ActiveJoystick.GetObjectPropertiesById((int) doi.ObjectType).SetRange(-5000, 5000);
            }

            ActiveJoystick.Properties.AxisMode = DeviceAxisMode.Absolute;
            ActiveJoystick.Acquire();

            JoystickState ActiveJoystickState = ActiveJoystick.GetCurrentState();

            int screenWidth = InternalGetSystemMetrics(0);
            int screenHeight = InternalGetSystemMetrics(1);

            int to_x = screenWidth / 2;
            int to_y = screenHeight / 2;

            int mic_x = (int) Math.Round(to_x * 65536.0 / screenWidth);
            int mic_y = (int) Math.Round(to_y * 65536.0 / screenHeight);

            int[] defaultDeviceState =
            {
                ActiveJoystickState.X,
                ActiveJoystickState.Y,
                ActiveJoystickState.RotationZ
            };

            INPUT[] MouseInputs = new INPUT[1];
            InputData KeyboardInput = new InputData();

            // Init Buttons

            int buttonCount = ActiveJoystick.Capabilities.ButtonCount;
            List<JoystickButton> joystickButtons = new List<JoystickButton>();
            int o = 0;
            for (o = 0; i < buttonCount; i++)
            {
                joystickButtons.Add(new JoystickButton()
                {
                    buttonName = "Button" + i,
                    buttonState = false
                });
            }

            bool[] buttonState = new bool[o];

            while (true)
            {
                loop:

                Thread.Sleep(100 / refreshRate);
                ActiveJoystickState = ActiveJoystick.GetCurrentState();

                buttonState = ActiveJoystickState.GetButtons();
                // poll buttons
                int e = 0;
                foreach (JoystickButton button in joystickButtons)
                {
                    button.buttonState = buttonState[e];
                    e++;
                }

                if (joystickButtons[0].buttonState)
                {
                    if (MouseInputs[0].U.mi.dwFlags != MOUSEEVENTF.LEFTDOWN)
                    {
                        MouseInputs[0].U.mi.dwFlags = MOUSEEVENTF.MOVE | MOUSEEVENTF.LEFTDOWN;
                    }
                    else
                    {
                        MouseInputs[0].U.mi.dwFlags = MOUSEEVENTF.MOVE;
                    }
                }
                else
                {
                    if (MouseInputs[0].U.mi.dwFlags > MOUSEEVENTF.LEFTDOWN)
                    {
                        MouseInputs[0].U.mi.dwFlags = MOUSEEVENTF.MOVE | MOUSEEVENTF.LEFTUP;
                    }
                    else
                    {
                        MouseInputs[0].U.mi.dwFlags = MOUSEEVENTF.MOVE;
                    }
                }

                if (ActiveJoystick.Poll().IsFailure ||
                    ActiveJoystick.GetCurrentState(ref ActiveJoystickState).IsFailure)
                {
                    Console.WriteLine("Polling Failed");
                    goto loop;
                }

                MouseInputs[0].U.mi.dx =
                    (int) ((ActiveJoystickState.X - defaultDeviceState[0]) * sensitivity * factor) *
                    (Settings.Default.InvertX ? -1 : 1);
                MouseInputs[0].U.mi.dy =
                    (int) ((ActiveJoystickState.Y - defaultDeviceState[1]) * sensitivity * factor) *
                    (Settings.Default.InvertY ? -1 : 1);
                MouseInputs[0].type = 0;
                SendInput(1, MouseInputs, INPUT.Size);



                if ((ActiveJoystick.GetCurrentState().RotationZ < 100 &&
                     ActiveJoystick.GetCurrentState().RotationZ > -100) && KeyboardInput.KEYEVENTF != KEYEVENTF.KEYUP)
                {
                    KeyboardInput.KEYEVENTF = KEYEVENTF.KEYUP;
                }

                if ((ActiveJoystick.GetCurrentState().RotationZ > defaultDeviceState[2] + 100) &&
                    KeyboardInput.ScanCodeShort != ScanCodeShort.KEY_D)
                {

                    KeyboardInput.KEYEVENTF = 0;
                    KeyboardInput.ScanCodeShort = ScanCodeShort.KEY_D;
                }

                if ((ActiveJoystick.GetCurrentState().RotationZ < defaultDeviceState[2] - 100) &&
                    KeyboardInput.ScanCodeShort != ScanCodeShort.KEY_A)
                {
                    KeyboardInput.KEYEVENTF = 0;
                    KeyboardInput.ScanCodeShort = ScanCodeShort.KEY_A;
                }

                SendInput(1, GetKeyInput(KeyboardInput.KEYEVENTF, KeyboardInput.ScanCodeShort), INPUT.Size);
            }
        }

        private static INPUT[] GetKeyInput(KEYEVENTF keyEvent, ScanCodeShort scanCode = 0)
        {
            INPUT[] KeyInput = new INPUT[1];
            KeyInput[0].type = 1;

            KeyInput[0].U.ki.wVk = 0;
            KeyInput[0].U.ki.dwFlags = keyEvent;
            KeyInput[0].U.ki.wScan = scanCode;

            return KeyInput;
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

        public class InputData
        {
            public ScanCodeShort ScanCodeShort { get; set; }
            public KEYEVENTF KEYEVENTF { get; set; }
        }

        public class JoystickButton
        {
            public string buttonName { get; set; }
            public bool buttonState { get; set; }
        }
    }
}
