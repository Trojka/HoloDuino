#include <WiFi101.h>
#include <AzureIoTHub.h>
#include <AzureIoTProtocol_MQTT.h>

#include "Iot.Secrets.h"
#include "SendToSerial.h"
#include "SendToAzure.h"
#include "samd/sample_init.h"

static char ssid[] = IOT_CONFIG_WIFI_SSID;
static char pass[] = IOT_CONFIG_WIFI_PASSWORD;

const int SensorPin = A0;
const unsigned long SendEveryMilliSeconds = 500;

unsigned long currentTime;
unsigned long lastMeasurementTime = 0;

int sensorValue;
float voltageValue;
float temperatureValue;

void setup() {

  sample_init(ssid, pass);
  
//    int status = WL_IDLE_STATUS;
//    Serial.print("Attempting to connect to WPA SSID: ");
//    Serial.println(ssid);
//
//    // Connect to WPA/WPA2 network. Change this line if using open or WEP network:
//    while ( status != WL_CONNECTED) 
//    {
//        // Connect to WPA/WPA2 network:
//        status = WiFi.begin(ssid, pass);
//        Serial.print("    WiFi status: ");
//        Serial.println(status);
//        // wait 2 seconds to try again
//        delay(2000);
//      }
//
//    Serial.println("\r\nConnected to wifi");
}

void loop() {
  // put your main code here, to run repeatedly:

  currentTime = millis();

  if ((currentTime - lastMeasurementTime) > SendEveryMilliSeconds)
  {
    CalcTemperature(&sensorValue, &voltageValue, &temperatureValue);
    //SendToSerial(sensorValue, voltageValue, temperatureValue);
    SendToAzure(sensorValue, voltageValue, temperatureValue);
    //SendToAzure(10, 20, 30);
    lastMeasurementTime = millis();
  }
  
}

void CalcTemperature(int* sensor, float* voltage, float* temperature) {
  *sensor = analogRead(SensorPin);

  *voltage = (*sensor/1024.0) * 3.3;  // The MKR1000 board operates at 3.3V 

  *temperature = (*voltage - .5) * 100;
}
