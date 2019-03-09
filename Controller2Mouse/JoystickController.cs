﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX.DirectInput;

namespace Controller2Mouse
{
    public class JoystickController
    {
        public ObservableCollection<DeviceInstance> ControllerList
        {
            get { return EnumerateDevices(); }
        }

        private DirectInput DirectInput;

        public JoystickController()
        {
            DirectInput = new DirectInput();
        }

        private ObservableCollection<DeviceInstance> EnumerateDevices()
        {
            Log.Add("Enumerating Devices...");

            var Devices = new ObservableCollection<DeviceInstance>();
            foreach (DeviceInstance device in DirectInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
                Devices.Add(device);

            switch (Devices.Count())
            {
                case 0:
                    Log.Add("No Devices Found");
                    break;
                case 1:
                    Log.Add(Devices.Count() + " Device Found");
                    break;
                default:
                    Log.Add(Devices.Count() + " Devices Found");
                    break;
            }

            return Devices;
        }

        public Joystick AcquireJoystick(DeviceInstance _DeviceInstance) => new Joystick(DirectInput, _DeviceInstance.InstanceGuid);
    }
}
