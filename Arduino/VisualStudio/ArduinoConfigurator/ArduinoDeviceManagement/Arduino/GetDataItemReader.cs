using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoDeviceManagement.Arduino
{
    public class GetDataItemReader
    {
        public GetDataItemReader(string commandIdentifier, string startMarker, string endMarker)
        {
            CommandIdentifier = commandIdentifier;
            StartMarker = startMarker;
            EndMarker = endMarker;
        }

        public string CommandIdentifier
        {
            get;
            private set;
        }

        public string StartMarker
        {
            get;
            private set;
        }

        public string EndMarker
        {
            get;
            private set;
        }

        public async Task<string> GetData(SerialPort port)
        {
            port.WriteLine(CommandIdentifier);

            byte[] buffer = new byte[1];
            string result = string.Empty;

            string receivedData = "";
            while (true)
            {
                await port.BaseStream.ReadAsync(buffer, 0, 1);
                receivedData += port.Encoding.GetString(buffer);

                if (receivedData.Contains(StartMarker) && receivedData.Contains(EndMarker))
                {
                    int startPos = receivedData.IndexOf(StartMarker) + StartMarker.Length;
                    int endPos = receivedData.IndexOf(EndMarker) - startPos;
                    var dataItem = receivedData.Substring(startPos, endPos);
                    return dataItem;
                }
            }
        }
    }
}
