#include <stdlib.h>

#include <stdio.h>
#include <stdint.h>

#include "AzureIoTHub.h"
#include "Iot.Secrets.h"
#include "Common.h"

static const char* connectionString = IOT_CONFIG_CONNECTION_STRING;

// Define the Model
BEGIN_NAMESPACE(PortToggle);

DECLARE_MODEL(ToggleModel,
WITH_ACTION(ToggleOn),
WITH_ACTION(ToggleOff)
);

END_NAMESPACE(PortToggle);

static char propText[1024];
static bool isPortOn = false;
static bool previousIsPortOn = true; // force setting the device twin on startup
static IOTHUB_CLIENT_LL_HANDLE g_iotHubClientHandle;
static ToggleModel* g_toggleModel;

EXECUTE_COMMAND_RESULT ToggleOn(ToggleModel* device)
{
    (void)device;
    //(void)printf("Turning port on.\r\n");
    digitalWrite(actionPin, 1);
    isPortOn = true;
    return EXECUTE_COMMAND_SUCCESS;
}

EXECUTE_COMMAND_RESULT ToggleOff(ToggleModel* device)
{
    (void)device;
    //(void)printf("Turning port off.\r\n");
    digitalWrite(actionPin, 0);
    isPortOn = false;
    return EXECUTE_COMMAND_SUCCESS;
}

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



static void deviceTwinCallback(DEVICE_TWIN_UPDATE_STATE update_state, const unsigned char* payLoad, size_t size, void* userContextCallback)
{
    (void)userContextCallback;

    printf("Device Twin update received (state=%s, size=%u): \r\n", 
        ENUM_TO_STRING(DEVICE_TWIN_UPDATE_STATE, update_state), size);
    for (size_t n = 0; n < size; n++)
    {
        printf("%c", payLoad[n]);
    }
    printf("\r\n");
}

static void reportedStateCallback(int status_code, void* userContextCallback)
{
    (void)userContextCallback;
    printf("Device Twin reported properties update completed with result: %d\r\n", status_code);

    //g_continueRunning = false;
}

#define PLATFORM_INIT_SUCCEEDED 1
#define SERIALIZER_INIT_SUCCEEDED 2
#define CREATEFROMCONNECTIONSTRING_SUCCEEDED 3
#define CREATE_MODEL_INSTANCE_SUCCEEDED 4

int initializationAt = 0;
bool InitToggleLogic()
{
    if (platform_init() != 0)
    {
        (void)printf("Failed to initialize platform.\r\n");
        return false;
    }
    else
    {
        initializationAt = PLATFORM_INIT_SUCCEEDED;
        if (serializer_init(NULL) != SERIALIZER_OK)
        {
            (void)printf("Failed on serializer_init\r\n");
            return false;
        }
        else
        {
            initializationAt = SERIALIZER_INIT_SUCCEEDED;
            g_iotHubClientHandle = IoTHubClient_LL_CreateFromConnectionString(connectionString, MQTT_Protocol);

            if (g_iotHubClientHandle == NULL)
            {
                (void)printf("Failed on IoTHubClient_LL_Create\r\n");
                return false;
            }
            else
            {
                initializationAt = CREATEFROMCONNECTIONSTRING_SUCCEEDED;
                g_toggleModel = CREATE_MODEL_INSTANCE(PortToggle, ToggleModel);
                if (g_toggleModel == NULL)
                {
                    (void)printf("Failed on CREATE_MODEL_INSTANCE\r\n");
                    return false;
                }
                else
                {
                    initializationAt = CREATE_MODEL_INSTANCE_SUCCEEDED;
                    if (IoTHubClient_LL_SetMessageCallback(g_iotHubClientHandle, IoTHubMessage, g_toggleModel) != IOTHUB_CLIENT_OK)
                    {
                        printf("unable to IoTHubClient_LL_SetMessageCallback\r\n");
                        return false;
                    }
                    if (IoTHubClient_LL_SetDeviceTwinCallback(g_iotHubClientHandle, deviceTwinCallback, g_iotHubClientHandle) != IOTHUB_CLIENT_OK)
                    {
                        printf("unable to IoTHubClient_LL_SetDeviceTwinCallback\r\n");
                        return false;
                    }
                    bool traceOn = true;
                    (void)IoTHubClient_LL_SetOption(g_iotHubClientHandle, OPTION_LOG_TRACE, &traceOn);

                    return true;
                }
            }
        }
    }
}

bool DestroyToggleLogic()
{
    if(initializationAt >= CREATE_MODEL_INSTANCE_SUCCEEDED)
        DESTROY_MODEL_INSTANCE(g_toggleModel);
    
    if(initializationAt >= CREATEFROMCONNECTIONSTRING_SUCCEEDED)
        IoTHubClient_LL_Destroy(g_iotHubClientHandle);
    
    if(initializationAt >= SERIALIZER_INIT_SUCCEEDED)
        serializer_deinit();
    
    if(initializationAt >= PLATFORM_INIT_SUCCEEDED)
        platform_deinit();  
}

void DoToggleLogic()
{
//    /* wait for commands */
//    bool doWork = true;
//    while (doWork)
//    {
//
//          resetButtonState = digitalRead(resetPin); 
//          if(resetButtonState == 1) {
//            doWork = false;
//          }
          
        if(previousIsPortOn != isPortOn)
        {
            if(isPortOn)
            {
                printf("setting port on in devicetwin");
                const char* reportedState = "{ 'toggle_state': 'on'}";
                size_t reportedStateSize = strlen(reportedState);
                (void)IoTHubClient_LL_SendReportedState(g_iotHubClientHandle, (const unsigned char*)reportedState, reportedStateSize, reportedStateCallback, g_iotHubClientHandle);
            }
            else
            {
                printf("setting port off in devicetwin");
                const char* reportedState = "{ 'toggle_state': 'off'}";
                size_t reportedStateSize = strlen(reportedState);
                (void)IoTHubClient_LL_SendReportedState(g_iotHubClientHandle, (const unsigned char*)reportedState, reportedStateSize, reportedStateCallback, g_iotHubClientHandle);
            }
            
            previousIsPortOn = isPortOn;
        }
        IoTHubClient_LL_DoWork(g_iotHubClientHandle);
        ThreadAPI_Sleep(100);
//    }  
}

void DoToggleLogic_AllInOne()
{
    if (platform_init() != 0)
    {
        (void)printf("Failed to initialize platform.\r\n");
    }
    else
    {
        if (serializer_init(NULL) != SERIALIZER_OK)
        {
            (void)printf("Failed on serializer_init\r\n");
        }
        else
        {
            IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle = IoTHubClient_LL_CreateFromConnectionString(connectionString, MQTT_Protocol);
            //srand((unsigned int)time(NULL));

            if (iotHubClientHandle == NULL)
            {
                (void)printf("Failed on IoTHubClient_LL_Create\r\n");
            }
            else
            {
                ToggleModel* toggleModel = CREATE_MODEL_INSTANCE(PortToggle, ToggleModel);
                if (toggleModel == NULL)
                {
                    (void)printf("Failed on CREATE_MODEL_INSTANCE\r\n");
                }
                else
                {
                    if (IoTHubClient_LL_SetMessageCallback(iotHubClientHandle, IoTHubMessage, toggleModel) != IOTHUB_CLIENT_OK)
                    {
                        printf("unable to IoTHubClient_SetMessageCallback\r\n");
                    }
                    else
                    {
                      
                        bool traceOn = true;

                        (void)IoTHubClient_LL_SetOption(iotHubClientHandle, OPTION_LOG_TRACE, &traceOn);
                        (void)IoTHubClient_LL_SetDeviceTwinCallback(iotHubClientHandle, deviceTwinCallback, iotHubClientHandle);

                        /* wait for commands */
                        bool doWork = true;
                        while (doWork)
                        {

                              resetButtonState = digitalRead(resetPin); 
                              if(resetButtonState == 1) {
                                doWork = false;
                              }
                              
                              if(previousIsPortOn != isPortOn)
                              {
                                if(isPortOn)
                                {
                                    printf("setting port on in devicetwin");
                                    const char* reportedState = "{ 'toggle_state': 'on'}";
                                    size_t reportedStateSize = strlen(reportedState);
                                    (void)IoTHubClient_LL_SendReportedState(iotHubClientHandle, (const unsigned char*)reportedState, reportedStateSize, reportedStateCallback, iotHubClientHandle);
                                }
                                else
                                {
                                    printf("setting port off in devicetwin");
                                    const char* reportedState = "{ 'toggle_state': 'off'}";
                                    size_t reportedStateSize = strlen(reportedState);
                                    (void)IoTHubClient_LL_SendReportedState(iotHubClientHandle, (const unsigned char*)reportedState, reportedStateSize, reportedStateCallback, iotHubClientHandle);
                                }
                                
                                previousIsPortOn = isPortOn;
                            }
                            IoTHubClient_LL_DoWork(iotHubClientHandle);
                            ThreadAPI_Sleep(100);
                        }
                    }

                    DESTROY_MODEL_INSTANCE(toggleModel);
                }
                IoTHubClient_LL_Destroy(iotHubClientHandle);
            }
            serializer_deinit();
        }
        platform_deinit();
    }
}


