//#include <FlashAsEEPROM.h>
#include <FlashStorage.h>

#include <WiFi101.h>
#include <AzureIoTHub.h>
#include <AzureIoTProtocol_MQTT.h>

#include "Iot.Secrets.h"
#include "SendToSerial.h"
#include "SendToAzure.h"
#include "samd/sample_init.h"
#include "DeviceConfig.h"

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


int wifiConnectLedPin = 6;
int wifiConnectLedState = LOW;

int serialConnectLedPin = 1;
int serialConnectLedState = LOW;

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

void setup() {

    pinMode(1, OUTPUT);
    pinMode(6, OUTPUT);

    digitalWrite(1, 0);
    digitalWrite(6, 0);

    Serial.begin(9600);  
    while (!Serial) ;

     

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

//    pinMode(serialConnectLedPin, OUTPUT);
//    pinMode(wifiConnectLedPin, OUTPUT);
//    
//    digitalWrite(serialConnectLedPin, serialConnectLedState);
//    digitalWrite(wifiConnectLedPin, wifiConnectLedState);

//    sample_init(ssid, pass);
  
    
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

String receivedString;

bool sendDeviceId = false;
bool sendDeviceDescription = false;

bool deviceIdWasSend = false;
bool deviceDescriptionWasSend = false;

String WifiSSIDMarker = "WIFISSID";
String WifiPWDMarker = "WIFIPWD";

bool setWifiSSID = false;
bool wifiSSIDWasSet = false;
bool setWifiPwd = false;
bool wifiPwdWasSet = false;

String setDataValue;
String wifiSSIDValue;
String wifiPwdValue;

void loop() {

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
    receivedString = "";
    setDataValue = "";
  }

//  if(wifiSSIDWasSet && wifiPwdWasSet)
//    digitalWrite(1, 1);

  while ((status != WL_CONNECTED) && wifiSSIDWasSet && wifiPwdWasSet) 
  {
      status = WiFi.begin(wifiSSIDValue.c_str(), wifiPwdValue.c_str());

      delay(2000);

      wifiConnectAttempt++;       
      if(wifiConnectAttempt > maxWifiConnectAttempts)
      {
        digitalWrite(6, 1);
        break;
      }
  }

  digitalWrite(1, 1);
  
  // put your main code here, to run repeatedly:

      //Serial.println("Wifi secrets valid !!!");
      //Serial.print("SSID: ");
      //Serial.println(wifiSecrets.ssid);
      //Serial.print("PWD: ");
      //Serial.println(wifiSecrets.pwd);

//if(wifiSecrets.valid == true) {
//  digitalWrite(1, 1); 
//}


//    char receiveVal; 
//    String receivedString;    
//
//    bool receivedSomething = false;
//    if(Serial.available() > 0)  
//    {          
//       receivedString = Serial.readString();
//       int indexOfR = receivedString.indexOf("\r");
//       receivedString.remove(indexOfR);
//       int indexOfN = receivedString.indexOf("\n");
//       receivedString.remove(indexOfN);
//       receivedSomething = true;
//
//       if(receivedSomething && !receivedWifiNetwork && !receivedWifiPwd)
//       {
//          ssidString = receivedString;
//
//          receivedWifiNetwork = true;
//          receivedSomething= false;
//       }
//
//       if(receivedSomething && receivedWifiNetwork && !receivedWifiPwd)
//       {
//          passString = receivedString;
//
//          receivedWifiPwd = true;
//          receivedSomething= false;
//       }
//       
//       if(receivedWifiNetwork && receivedWifiPwd)      
//          serialConnectLedState = HIGH;    
//
//       digitalWrite(serialConnectLedPin, serialConnectLedState); 
//       Serial.println("Received [" + receivedString + "]");
//       Serial.flush();
//    }  
//
//
//    while ((status != WL_CONNECTED) && receivedWifiNetwork && receivedWifiPwd) 
//    {
//        Serial.println("Not yet conected to WIFI");
//        
//        Serial.println("WiFi ntw: " + ssidString);
//        Serial.flush();
//
//        Serial.println("WiFi pwd: " + passString);
//        Serial.flush();
//        
//        // Connect to WPA/WPA2 network:
//        status = WiFi.begin(ssidString.c_str(), passString.c_str());
//        Serial.println("WiFi status: " + ((status == WL_CONNECTED) ? String("connected") : String("not connected")));
//        Serial.flush();
//        
//        Serial.print("Try again...");
//        Serial.flush();
//
//        // wait 2 seconds to try again
//        delay(2000);
//
//
//        wifiConnectAttempt++;
//
//        Serial.println("Attemp: " + String(wifiConnectAttempt));
//        Serial.flush();
//          
//        if(wifiConnectAttempt > maxWifiConnectAttempts)
//        {
//          Serial.println("Maximum attempts for wifi connection exceeded.");
//          Serial.flush();
//          break;
//        }
//    }
//
//    if((status == WL_CONNECTED) && receivedWifiNetwork && receivedWifiPwd)
//    {
//      wifiConnectLedState = HIGH;   
//      digitalWrite(wifiConnectLedPin, wifiConnectLedState); 
//      IPAddress ip = WiFi.localIP();
//      Serial.println("Conected to WIFI: " + ip);
//      Serial.flush();
//    }
//    else
//    {
//      serialConnectLedState = LOW;  
//      digitalWrite(serialConnectLedPin, serialConnectLedState); 
//    }
//      
//        
//    delay(50); 
  
  

//  currentTime = millis();
//
//  if ((currentTime - lastMeasurementTime) > SendEveryMilliSeconds)
//  {
//    CalcTemperature(&sensorValue, &voltageValue, &temperatureValue);
//    //SendToSerial(sensorValue, voltageValue, temperatureValue);
//    SendToAzure(sensorValue, voltageValue, temperatureValue);
//    //SendToAzure(10, 20, 30);
//    lastMeasurementTime = millis();
//  }
  
}

void CalcTemperature(int* sensor, float* voltage, float* temperature) {
  *sensor = analogRead(SensorPin);

  *voltage = (*sensor/1024.0) * 3.3;  // The MKR1000 board operates at 3.3V 

  *temperature = (*voltage - .5) * 100;
}
