#include <Arduino.h>
#include "DeviceConfig.h"


void SendDeviceId() {
  String deviceId = "[DEVICEID]087E94E8-5A0F-43A9-B20F-B17CC3CBC9C5[END]";
  Serial.println(deviceId);
}

void SendDeviceConfig() {
  String descriptor = "[DEVICEDESCRIPTION]"
  "{\"systems\":{"
  "\"name\":\"Buttons\","
  "\"count\":2,"
  "\"sys1\":{"
    "\"id\":1,"
    "\"ui\":{"
      "\"type\":\"toggle\","
      "\"actions\":{"
        "\"count\":2,"
        "\"action1\":{\"event\":\"toggleon\",\"method\":\"1_on\"},"
        "\"action2\":{\"event\":\"toggleoff\",\"method\":\"1_off\"}"
      "}"
    "}}"
//  ",\"sys2\":{"
//    "\"id\":2,"
//    "\"ui\":{"
//      "\"type\":\"toggle\","
//      "\"actions\":{"
//        "\"count\":2,"
//        "\"action1\":{\"event\":\"toggleon\",\"method\":\"2_on\"},"
//        "\"action2\":{\"event\":\"toggleoff\",\"method\":\"2_off\"}"
//      "}"
//    "}}"
  "}}[END]";
  Serial.println(descriptor);
}
