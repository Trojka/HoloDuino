#include <Arduino.h>
#include "DeviceConfig.h"

bool startSendingReceived = false;

void SendDeviceId() {
  String deviceId = "[DEVICEID]087E94E8-5A0F-43A9-B20F-B17CC3CBC9C5[END]";
  Serial.println(deviceId);
}

void SendDeviceConfig() {
  String descriptor = "[DEVICEDESCRIPTION]"
  "{\"systems\":{"
  "\"count\":2,"
  "\"sys1\":{"
    "\"id\":1,"
    "\"ui\":{"
      "\"type\":\"toggle\","
      "\"actions\":{"
        "\"count\":2,"
        "\"action1\":{\"event\":\"on\",\"method\":\"1_on\"},"
        "\"action2\":{\"event\":\"on\",\"method\":\"1_on\"}"
      "}"
    "}},"
  "\"sys2\":{"
    "\"id\":2,"
    "\"ui\":{"
      "\"type\":\"toggle\","
      "\"actions\":{"
        "\"count\":2,"
        "\"action1\":{\"event\":\"on\",\"method\":\"2_on\"},"
        "\"action2\":{\"event\":\"on\",\"method\":\"2_on\"}"
      "}"
    "}}"
  "}}[END]";
  Serial.println(descriptor);
}
