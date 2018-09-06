using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoDeviceManagement.Arduino
{
    public class DeviceDescriptionReader : GetDataItemReader
    {
        public DeviceDescriptionReader()
            : base("DEVICEDESCRIPTIONGET", "[DEVICEDESCRIPTION]", "[END]")
        {

        }
    }
}

