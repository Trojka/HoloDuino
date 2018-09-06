using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoDeviceManagement.Arduino
{
    public class DeviceIdReader : GetDataItemReader
    {
        public DeviceIdReader()
            : base("DEVICEIDGET", "[DEVICEID]", "[END]")
        {

        }
    }
}
