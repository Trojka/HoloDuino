#include <stdlib.h>

#include <stdio.h>
#include <stdint.h>

#include "AzureIoTHub.h"
#include "Iot.Secrets.h"

// Define the Model
BEGIN_NAMESPACE(TempMeasurement);

DECLARE_MODEL(Measurement,
WITH_DATA(int, SensorValue),
WITH_DATA(float, Voltage),
WITH_DATA(float, Temperature)
);

END_NAMESPACE(TempMeasurement);


void SendToAzure(int sensor, float voltage, float temperature) {

    if (platform_init() != 0)
    {
        (void)printf("Failed to initialize platform.\r\n");
        return;
    }

    if (serializer_init(NULL) != SERIALIZER_OK)
    {
        (void)printf("Failed on serializer_init\r\n");
        return;
    }

    IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle = IoTHubClient_LL_CreateFromConnectionString(connectionString, MQTT_Protocol);
    if (iotHubClientHandle == NULL)
    {
        (void)printf("Failed on IoTHubClient_LL_Create\r\n");
        serializer_deinit();
        platform_deinit();
        return;
    }

}
