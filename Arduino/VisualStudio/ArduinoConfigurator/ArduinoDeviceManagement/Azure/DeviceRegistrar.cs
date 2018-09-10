using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
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

        public async void RegisterDevice(string deviceId, string deviceDescription)
        {
            var tags = "{\"tags\":" + deviceDescription + "}";
            var primaryKey = CryptoKeyGenerator.GenerateKey(32);
            var secondaryKey = CryptoKeyGenerator.GenerateKey(32);

            var newDevice = new Device(deviceId);
            newDevice.Authentication = new AuthenticationMechanism();

            newDevice.Authentication.SymmetricKey.PrimaryKey = primaryKey;
            newDevice.Authentication.SymmetricKey.SecondaryKey = secondaryKey;

            var device = await registryManager.AddDeviceAsync(newDevice);

            Type twinType = typeof(Twin);
            var deviceTwin = await registryManager.GetTwinAsync(deviceId);
            dynamic dp = JsonConvert.DeserializeObject(tags, twinType);
            dp.DeviceId = deviceId;
            dp.ETag = deviceTwin.ETag;
            await registryManager.UpdateTwinAsync(dp.DeviceId, dp, dp.ETag);

        }
    }
}
