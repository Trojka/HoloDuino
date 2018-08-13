#include <WiFi101.h>
#include <AzureIoTHub.h>
#include <AzureIoTProtocol_MQTT.h>

#include "Iot.Secrets.h"
#include "SendToSerial.h"
#include "SendToAzure.h"
#include "samd/sample_init.h"

static char ssid[] = IOT_CONFIG_WIFI_SSID;
static char pass[] = IOT_CONFIG_WIFI_PASSWORD;

String ssidString;
String passString;

const int SensorPin = A0;
const unsigned long SendEveryMilliSeconds = 500;

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

void setup() {

    Serial.begin(9600);  

    pinMode(serialConnectLedPin, OUTPUT);
    pinMode(wifiConnectLedPin, OUTPUT);
    
    digitalWrite(serialConnectLedPin, serialConnectLedState);
    digitalWrite(wifiConnectLedPin, wifiConnectLedState);

//  sample_init(ssid, pass);
  
    
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


    char receiveVal; 
    String receivedString;    
     
    if(Serial.available() > 0)  
    {          
       receivedString = Serial.readString();

       if(!receivedWifiNetwork && !receivedWifiPwd)
       {
          //ssidString = new char [receivedString.length()+1];
          //strcpy (ssidString, receivedString.c_str());
          ssidString = receivedString;

          receivedWifiNetwork = true;
       }

       if(receivedWifiNetwork && !receivedWifiPwd)
       {
          //passString = new char [receivedString.length()+1];
          //strcpy (passString, receivedString.c_str());
          passString = receivedString;

          receivedWifiPwd = true;
       }
       
       if(receivedWifiNetwork && receivedWifiPwd)      
          serialConnectLedState = HIGH;    

       digitalWrite(serialConnectLedPin, serialConnectLedState); 
       Serial.println(receivedString);
       Serial.flush();
    }  


    while ((status != WL_CONNECTED) && receivedWifiNetwork && receivedWifiPwd) 
    {
        Serial.println("Not yet conected to WIFI");
        
        Serial.println("WiFi " + String(ssid));
        //Serial.println(ssidString);
        Serial.flush();

        Serial.print("WiFi pwd " + String(pass));
        //Serial.println(passString);
        Serial.flush();
        
        // Connect to WPA/WPA2 network:
        status = WiFi.begin(ssid, pass);
        Serial.println("WiFi status: " + ((status == WL_CONNECTED) ? String("connected") : String("not connected")));
        //Serial.println((status == WL_CONNECTED) ? "connected" : "not connected");
        Serial.flush();
        
        Serial.print("Try again...");
        Serial.flush();

        // wait 2 seconds to try again
        delay(2000);


        wifiConnectAttempt++;

        Serial.println("Attemp: " + String(wifiConnectAttempt));
        //Serial.println(String(wifiConnectAttempt));
        Serial.flush();
          
        if(wifiConnectAttempt > maxWifiConnectAttempts)
        {
          Serial.println("Maximum attempts for wifi connection exceeded.");
          Serial.flush();
          break;
        }
    }

    if((status == WL_CONNECTED) && receivedWifiNetwork && receivedWifiPwd)
    {
      wifiConnectLedState = HIGH;   
      digitalWrite(wifiConnectLedPin, wifiConnectLedState); 
      IPAddress ip = WiFi.localIP();
      Serial.println("Conected to WIFI: " + ip);
      Serial.flush();
    }
    else
    {
      serialConnectLedState = LOW;  
      digitalWrite(serialConnectLedPin, serialConnectLedState); 
    }
      
        
    delay(50); 
  
  

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
