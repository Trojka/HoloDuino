using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoDeviceManagement
{
    public class ComPort
    {
        public string Id { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"({Id}, {Description})";
        }
    }
}
