using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoDeviceManagement.Azure
{
    public class DeviceRegistrar
    {
        RegistryManager registryManager;

        public DeviceRegistrar(string connectionString)
        {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }

        public async void RegisterDevice(string deviceId)
        {
            var primaryKey = CryptoKeyGenerator.GenerateKey(32);
            var secondaryKey = CryptoKeyGenerator.GenerateKey(32);

            var device = new Device(deviceId);
            device.Authentication = new AuthenticationMechanism();

            device.Authentication.SymmetricKey.PrimaryKey = primaryKey;
            device.Authentication.SymmetricKey.SecondaryKey = secondaryKey;

            await registryManager.AddDeviceAsync(device);

        }
    }
}
