using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX.DirectInput;

namespace Controller2Mouse
{
    class JoystickController
    {
        public ObservableCollection<DeviceInstance> ControllerList
        {
            get { return EnumerateDevices() as ObservableCollection<DeviceInstance>; }
        }

        public JoystickController()
        {
            
        }

        private ObservableCollection<DeviceInstance> EnumerateDevices()
        {
            ObservableCollection<DeviceInstance> Devices = new ObservableCollection<DeviceInstance>();
            DirectInput Input = new DirectInput();

            IList<DeviceInstance> List = new DirectInput().GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);
            foreach (DeviceInstance device in List)
                Devices.Add(device);
            return Devices;
        }
    }
}
