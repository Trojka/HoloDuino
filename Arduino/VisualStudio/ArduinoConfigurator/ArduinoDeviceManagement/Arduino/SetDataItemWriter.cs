using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoDeviceManagement.Arduino
{
    public class SetDataItemWriter
    {
        public SetDataItemWriter(string commandIdentifier, string dataMarker)
        {
            CommandIdentifier = commandIdentifier;
            DataMarker = dataMarker;
        }

        public string CommandIdentifier
        {
            get;
            private set;
        }

        public string DataMarker
        {
            get;
            private set;
        }

        public void SetData(SerialPort port, string dataValue)
        {
            port.WriteLine(DataMarker + dataValue + CommandIdentifier);
        }
    }
}
