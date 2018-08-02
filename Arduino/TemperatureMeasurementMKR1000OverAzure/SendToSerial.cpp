#include <Arduino.h>
#include "SendToSerial.h"

void SendToSerial(int sensor, float voltage, float temperature) {
  Serial.print("Sensor Value: ");
  Serial.print(sensor);
  //Serial.println();

  Serial.print(", Volts: ");
  Serial.print(voltage);
  //Serial.println();

  Serial.print(", degrees C: ");
  Serial.print(temperature);
  Serial.println();
}
