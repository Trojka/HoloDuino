//#include <FlashAsEEPROM.h>
#include <FlashStorage.h>

#include <WiFi101.h>
#include <AzureIoTHub.h>
#include <AzureIoTProtocol_MQTT.h>

#include "Iot.Secrets.h"
#include "ToggleLogic.h"
#include "samd/sample_init.h"
#include "DeviceConfig.h"
//#include "Common.h"


static char ssid[] = IOT_CONFIG_WIFI_SSID;
static char pass[] = IOT_CONFIG_WIFI_PASSWORD;

//String ssidString = "WIFI_SSID";
//String passString = "WIFI_PWD";

const int SensorPin = A0;
const unsigned long SendEveryMilliSeconds = 5000;

unsigned long currentTime;
unsigned long lastMeasurementTime = 0;

int sensorValue;
float voltageValue;
float temperatureValue;


int wifiConnectLedPin = 2;
int wifiConnectLedState = LOW;
int wifiDisconnectLedPin = 1;
int wifiDisconnectLedState = LOW;

int receivingBlinkTimeOut = 5;

bool receivedWifiNetwork = false;
bool receivedWifiPwd = false;
const int maxWifiConnectAttempts = 10;
int wifiConnectAttempt = 0;
int status = WL_IDLE_STATUS;

typedef struct {
  boolean valid;
  char ssid[50];
  char pwd[50];
} WifiSecrets;

//WifiSecrets wifiSecrets;
//FlashStorage(_storedWifiSecrets, WifiSecrets);


int actionPin = 6;
int actionState = 0;

int resetPin = 5;
int resetButtonState = 0;


#define MIN_EPOCH 40 * 365 * 24 * 3600


void setup() {

    pinMode(wifiConnectLedPin, OUTPUT);
    pinMode(wifiDisconnectLedPin, OUTPUT);
    pinMode(resetPin, INPUT);
    
    pinMode(actionPin, OUTPUT);

    wifiConnectAttempt = 0;
  


    Serial.begin(9600);  
    

//    wifiSecrets = _storedWifiSecrets.read();
//
//    if(wifiSecrets.valid == false) {
//      //Serial.println("Wifi secrets invalid !!!");
//      
//      ssidString.toCharArray(wifiSecrets.ssid, 50);
//      passString.toCharArray(wifiSecrets.pwd, 50);
//      wifiSecrets.valid = true;
//
//      //_storedWifiSecrets.write(wifiSecrets);
//
//      //Serial.print("SSID: ");
//      //Serial.println(wifiSecrets.ssid);
//      //Serial.print("PWD: ");
//      //Serial.println(wifiSecrets.pwd);
//
//
//      //digitalWrite(1, 0); 
//      digitalWrite(6, 1); 
//    }
//    else {
//      //Serial.println("Wifi secrets valid !!!");
//      //Serial.print("SSID: ");
//      //Serial.println(wifiSecrets.ssid);
//      //Serial.print("PWD: ");
//      //Serial.println(wifiSecrets.pwd);
//
//      //digitalWrite(1, 1);
//      digitalWrite(6, 0); 
//    }


}

// Azure IoT samples contain their own loops, so only run them once
static bool done = false;
void loop_hide() {
    if (!done)
    {
        DoToggleLogic();
        done = true;
    }
    else
    {
      delay(500);
    }
}


String receivedString;

bool sendDeviceId = false;
bool sendDeviceDescription = false;

bool deviceIdWasSend = false;
bool deviceDescriptionWasSend = false;

String WifiSSIDMarker = "WIFISSID";
String WifiPWDMarker = "WIFIPWD";

bool setWifiSSID = false;
bool wifiSSIDWasSet = false;
bool confirmSSID = false;
bool setWifiPwd = false;
bool wifiPwdWasSet = false;
bool confirmPWD = false;

String setDataValue;
String wifiSSIDValue;
String wifiPwdValue;

int receivedDataItemFlashDuration = -1;
int lastFlashToggle;
bool flashState = false;

void loop() {

  resetButtonState = digitalRead(resetPin); 
  if(resetButtonState == HIGH) {
    wifiSSIDWasSet = false;
    wifiPwdWasSet = false;
  }


  if(Serial.available() > 0) {
    String data = Serial.readString();
    receivedString = receivedString + data;

    int endOfGet = receivedString.indexOf("GET");
    if(endOfGet != -1) {
      String command = receivedString.substring(0, endOfGet);
      if(command.equals("DEVICEID"))
        sendDeviceId = true;
      if(command.equals("DEVICEDESCRIPTION"))
        sendDeviceDescription = true;
    }

    int startOfSet = receivedString.indexOf("SET");
    if(startOfSet != -1) {
      //digitalWrite(6, 1);   
      String command = receivedString.substring(0, startOfSet);
      if(command.indexOf(WifiSSIDMarker) != -1)
      {
        setDataValue = command;
        setWifiSSID = true;
      }
      if(command.indexOf(WifiPWDMarker) != -1)
      {
        setDataValue = command;
        setWifiPwd = true;
      }
    }
  }

  if(sendDeviceId && !deviceIdWasSend) {
    SendDeviceId();
    //digitalWrite(1, 1);
    deviceIdWasSend = true;
    receivedString = "";
  }

  if(sendDeviceDescription && !deviceDescriptionWasSend) {
    SendDeviceConfig();
    //digitalWrite(1, 1);
    deviceDescriptionWasSend = true;
    receivedString = "";
  }

  if(setWifiSSID && !wifiSSIDWasSet)
  {
    int wifiSSIDMarkerStart = setDataValue.indexOf(WifiSSIDMarker);
    int wifiSSIDDataStart = wifiSSIDMarkerStart + WifiSSIDMarker.length() + 1;
    wifiSSIDValue = setDataValue.substring(wifiSSIDDataStart);
    //digitalWrite(1, 1);

    wifiSSIDWasSet = true;
    confirmSSID = true;
    receivedDataItemFlashDuration = 800;
    lastFlashToggle = millis();

    receivedString = "";
    setDataValue = "";
  }

  if(setWifiPwd && !wifiPwdWasSet)
  {
    int wifiPWDMarkerStart = setDataValue.indexOf(WifiPWDMarker);
    int wifiPWDDataStart = wifiPWDMarkerStart + WifiPWDMarker.length() + 1;
    wifiPwdValue = setDataValue.substring(wifiPWDDataStart);
    //digitalWrite(1, 1);

    wifiPwdWasSet = true;
    confirmPWD = true;
    receivedDataItemFlashDuration = 400;
    lastFlashToggle = millis();
    
    receivedString = "";
    setDataValue = "";
  }

//  if(wifiSSIDWasSet && wifiPwdWasSet)
//    digitalWrite(1, 1);

if(wifiSSIDWasSet && confirmSSID)
{
  //Serial.print("SSID was set: ");
  //Serial.println(wifiSSIDValue.c_str());
  Serial.println(WifiSSIDMarker + "CONFIRM");
  confirmSSID = false;
}
//else
//  Serial.println("SSID was NOT set");

  
if(wifiPwdWasSet && confirmPWD)
{
  //Serial.print("PWD was set: ");
  //Serial.println(wifiPwdValue.c_str());
  Serial.println(WifiPWDMarker + "CONFIRM");
  confirmPWD = false;
}
//else
//  Serial.println("PWD was NOT set");

  if(receivedDataItemFlashDuration != -1 && (millis() - lastFlashToggle > receivedDataItemFlashDuration))
  {
    flashState = !flashState;
    digitalWrite(wifiConnectLedPin, flashState?1:0);
    lastFlashToggle = millis();
  }


  //wifiConnectAttempt = 0;
  while (wifiSSIDWasSet && wifiPwdWasSet && (status != WL_CONNECTED) && (wifiConnectAttempt < maxWifiConnectAttempts))
  {
    receivedDataItemFlashDuration = -1;
      Serial.println("attempt to connect");
      status = WiFi.begin(wifiSSIDValue.c_str(), wifiPwdValue.c_str());

      delay(200);

      flashState = !flashState;
      digitalWrite(wifiConnectLedPin, flashState?1:0);

      wifiConnectAttempt++;       
      if(wifiConnectAttempt >= maxWifiConnectAttempts)
      {
        digitalWrite(wifiConnectLedPin, 0);
        digitalWrite(wifiDisconnectLedPin, 1);
        Serial.println("attempt to connect failed");
        
        break;
      }
  }

  if((status == WL_CONNECTED) && wifiSSIDWasSet && wifiPwdWasSet)
  {
    Serial.println("connected");
    
    digitalWrite(wifiConnectLedPin, 1);
    digitalWrite(wifiDisconnectLedPin, 0);

    sample_inittime();
    //DoToggleLogic();
  }

}

