using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class DeviceRegistry
{
    public DeviceRegistry()
    {
        Devices = new List<DeviceModel>();
    }

    public List<DeviceModel> Devices { get; set; }
}

