#include <Arduino.h>
#include "DeviceConfig.h"

bool startSendingReceived = false;

void SendDeviceId() {
  String deviceId = "[DEVICEID]087E94E8-5A0F-43A9-B20F-B17CC3CBC9C5[END]";
  Serial.println(deviceId);
}

void SendDeviceConfig() {
  String descriptor = "[DEVICEDESCRIPTION]<subsystems>"
  "<system id='1'>"
    "<ui>"
      "<toggle>"
        "<action event='on' method='1_on'/>"
        "<action event='off' method='1_off'/>"
      "</toggle>"
    "</ui>"
  "</system>"
  "<system id='2'>"
    "<ui>"
      "<toggle>"
        "<action event='on' method='2_on'/>"
        "<action event='off' method='2_off'/>"
      "</toggle>"
    "</ui>"
  "</system>"
  "</subsystems>[END]";
  Serial.println(descriptor);
}
