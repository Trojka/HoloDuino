using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class DeviceIdReceiver
    {
        bool receivingData = false;
        string receivedData = "";

        public string GetDeviceId(string serialPort)
        {
            string result = string.Empty;

            SerialPort port = new SerialPort(serialPort, 9600); //, Parity.None, 8, StopBits.One);
            port.Open();

            receivingData = false;
            receivedData = "";

            port.DataReceived += DataReceived;

            port.WriteLine("DEVICEIDGET");

            return result;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            var data = port.ReadExisting();
            receivedData = receivedData + data;

            if (receivedData.Contains("[DEVICEID]") && receivedData.Contains("[END]"))
            {
                var deviceId = receivedData.Substring("[DEVICEID]".Length, receivedData.IndexOf("[END]") - "[DEVICEID]".Length);
                Debug.WriteLine(deviceId);
            }
        }
    }
}
