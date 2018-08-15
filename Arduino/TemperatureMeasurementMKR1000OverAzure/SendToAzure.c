#include <stdlib.h>

#include <stdio.h>
#include <stdint.h>

#include "AzureIoTHub.h"
#include "Iot.Secrets.h"

static const char* connectionString = IOT_CONFIG_CONNECTION_STRING;


// Define the Model
BEGIN_NAMESPACE(TempMeasurement);

DECLARE_MODEL(Measurement,
WITH_DATA(int, SensorValue),
WITH_DATA(float, Voltage),
WITH_DATA(float, Temperature),
WITH_ACTION(TurnLedOn),
WITH_ACTION(TurnLedOff)
);

END_NAMESPACE(TempMeasurement);


EXECUTE_COMMAND_RESULT TurnLedOn(Measurement* device)
{
    (void)device;
    (void)printf("Turning LED on.\r\n");
    digitalWrite(1, 1); 
    return EXECUTE_COMMAND_SUCCESS;
}

EXECUTE_COMMAND_RESULT TurnLedOff(Measurement* device)
{
    (void)device;
    (void)printf("Turning LED off.\r\n");
    digitalWrite(1, 0); 
    return EXECUTE_COMMAND_SUCCESS;
}

void sendCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result, void* userContextCallback)
{
    unsigned int messageTrackingId = (unsigned int)(uintptr_t)userContextCallback;

    printf("Message Id: %u Received.\r\n", messageTrackingId);

    printf("Result Call Back Called! Result is: %s \r\n", ENUM_TO_STRING(IOTHUB_CLIENT_CONFIRMATION_RESULT, result));
}


static void sendMessage(IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle, const unsigned char* buffer, size_t size, Measurement *myMeasurement)
{
    static unsigned int messageTrackingId;
    IOTHUB_MESSAGE_HANDLE messageHandle = IoTHubMessage_CreateFromByteArray(buffer, size);
    if (messageHandle == NULL)
    {
        printf("unable to create a new IoTHubMessage\r\n");
    }
    else
    {
//        MAP_HANDLE propMap = IoTHubMessage_Properties(messageHandle);
//        (void)sprintf_s(propText, sizeof(propText), myWeather->Temperature > 28 ? "true" : "false");
//        if (Map_AddOrUpdate(propMap, "temperatureAlert", propText) != MAP_OK)
//        {
//            (void)printf("ERROR: Map_AddOrUpdate Failed!\r\n");
//        }

        if (IoTHubClient_LL_SendEventAsync(iotHubClientHandle, messageHandle, sendCallback, (void*)(uintptr_t)messageTrackingId) != IOTHUB_CLIENT_OK)
        {
            printf("failed to hand over the message to IoTHubClient");
        }
        else
        {
            printf("IoTHubClient accepted the message for delivery\r\n");
        }
        IoTHubMessage_Destroy(messageHandle);
    }
    messageTrackingId++;
}


/*this function "links" IoTHub to the serialization library*/
static IOTHUBMESSAGE_DISPOSITION_RESULT IoTHubMessage(IOTHUB_MESSAGE_HANDLE message, void* userContextCallback)
{
    IOTHUBMESSAGE_DISPOSITION_RESULT result;
    const unsigned char* buffer;
    size_t size;
    if (IoTHubMessage_GetByteArray(message, &buffer, &size) != IOTHUB_MESSAGE_OK)
    {
        printf("unable to IoTHubMessage_GetByteArray\r\n");
        result = IOTHUBMESSAGE_ABANDONED;
    }
    else
    {
        /*buffer is not zero terminated*/
        char* temp = malloc(size + 1);
        if (temp == NULL)
        {
            printf("failed to malloc\r\n");
            result = IOTHUBMESSAGE_ABANDONED;
        }
        else
        {
            (void)memcpy(temp, buffer, size);
            temp[size] = '\0';
            EXECUTE_COMMAND_RESULT executeCommandResult = EXECUTE_COMMAND(userContextCallback, temp);
            result =
                (executeCommandResult == EXECUTE_COMMAND_ERROR) ? IOTHUBMESSAGE_ABANDONED :
                (executeCommandResult == EXECUTE_COMMAND_SUCCESS) ? IOTHUBMESSAGE_ACCEPTED :
                IOTHUBMESSAGE_REJECTED;
            free(temp);
        }
    }
    return result;
}

void SendToAzure(int sensor, float voltage, float temperature) {

    if (platform_init() != 0)
    {
        printf("Failed to initialize platform.\r\n");
    }
    else
    {
      if (serializer_init(NULL) != SERIALIZER_OK)
      {
          printf("Failed on serializer_init\r\n");
      }
      else
      {
        IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle = IoTHubClient_LL_CreateFromConnectionString(connectionString, MQTT_Protocol);
        if (iotHubClientHandle == NULL)
        {
            printf("Failed on IoTHubClient_LL_Create\r\n");
        }
        else
        {
          Measurement* someMeasurement = CREATE_MODEL_INSTANCE(TempMeasurement, Measurement);
          if(someMeasurement == NULL)
          {
            printf("Failed on CREATE_MODEL_INSTANCE(TempMeasurement, Measurement)\r\n");
          }
          else
          {

            if (IoTHubClient_LL_SetMessageCallback(iotHubClientHandle, IoTHubMessage, someMeasurement) != IOTHUB_CLIENT_OK)
            {
                printf("unable to IoTHubClient_SetMessageCallback\r\n");
            }
            else
            {
            
              someMeasurement->SensorValue = sensor;
              someMeasurement->Voltage = voltage;
              someMeasurement->Temperature = temperature;

              {
//                unsigned char* destination;
//                size_t destinationSize;
//                if (SERIALIZE(&destination, &destinationSize, someMeasurement->SensorValue, someMeasurement->Voltage, someMeasurement->Temperature) != CODEFIRST_OK)
//                {
//                    printf("Failed to serialize\r\n");
//                }
//                else
//                {
//                    sendMessage(iotHubClientHandle, destination, destinationSize, someMeasurement);
//                    free(destination);
//                }
              }

//              /* wait for commands */
//              while (1)
//              {
//                  IoTHubClient_LL_DoWork(iotHubClientHandle);
//                  ThreadAPI_Sleep(100);
//              }

                IOTHUB_CLIENT_STATUS status;

                printf("Getting Send Status\r\n");
                while (1) //(IoTHubClient_LL_GetSendStatus(iotHubClientHandle, &status) == IOTHUB_CLIENT_OK) && (status == IOTHUB_CLIENT_SEND_STATUS_BUSY))
                {
                    printf("Do Work\r\n");
                    IoTHubClient_LL_DoWork(iotHubClientHandle);
                    ThreadAPI_Sleep(100);
                }              
              
            }         
            
            DESTROY_MODEL_INSTANCE(someMeasurement);
          }

          IoTHubClient_LL_Destroy(iotHubClientHandle);
          
        }

        serializer_deinit();

      }
      
      platform_deinit();
      
    }

}
