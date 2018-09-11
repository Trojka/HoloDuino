using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoDeviceManagement.Arduino
{
    public class ArduinoComm
    {
        public static List<ComPort> GetComPorts()
        {
            List<ComPort> result = new List<ComPort>();

            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    result.Add(new ComPort() { Id = deviceId, Description = desc });
                }
            }
            catch (ManagementException e)
            {
                /* Do Nothing */
            }

            return result;
        }

        // https://stackoverflow.com/questions/3293889/how-to-auto-detect-arduino-com-port
        public static string AutodetectArduinoPortId()
        {
            try
            {
                foreach (var port in GetComPorts())
                {
                    if (port.Description.Contains("Arduino"))
                    {
                        return port.Id;
                    }
                }
            }
            catch (ManagementException e)
            {
                /* Do Nothing */
            }

            return null;
        }
    }
}
