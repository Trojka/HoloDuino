#include <Arduino.h>
#include "DeviceConfig.h"

bool startSendingReceived = false;

void SendDeviceId() {


  
}




void DeviceConfig() {
  String descriptor = "<subsystems>"
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
  "</subsystems>";
}
