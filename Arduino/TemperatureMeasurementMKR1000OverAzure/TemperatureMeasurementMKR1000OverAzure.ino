#include <AzureIoTHub.h>
#include <AzureIoTProtocol_MQTT.h>

#include "SendToSerial.h"
#include "SendToAzure.h"

const int SensorPin = A0;
const unsigned long SendEveryMilliSeconds = 10000;

unsigned long currentTime;
unsigned long lastMeasurementTime = 0;

int sensorValue;
float voltageValue;
float temperatureValue;

void setup() {
  // put your setup code here, to run once:

}

void loop() {
  // put your main code here, to run repeatedly:

  currentTime = millis();

  if ((currentTime - lastMeasurementTime) > SendEveryMilliSeconds)
  {
    CalcTemperature(&sensorValue, &voltageValue, &temperatureValue);
    SendToSerial(sensorValue, voltageValue, temperatureValue);
    SendToAzure(sensorValue, voltageValue, temperatureValue);
    lastMeasurementTime = millis();
  }
  
}

void CalcTemperature(int* sensor, float* voltage, float* temperature) {
  *sensor = analogRead(SensorPin);

  *voltage = (*sensor/1024.0) * 3.3;  // The MKR1000 board operates at 3.3V 

  *temperature = (*voltage - .5) * 100;
}
