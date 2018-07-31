const int LedPin1 = 4;
const int LedPin2 = 3;
const int LedPin3 = 2;

const int SensorPin = A0;


void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);

  pinMode(LedPin1, OUTPUT);
  digitalWrite(LedPin1, LOW);

  pinMode(LedPin2, OUTPUT);
  digitalWrite(LedPin2, LOW);

  pinMode(LedPin3, OUTPUT);
  digitalWrite(LedPin3, LOW);
  
}

void loop() {
  // put your main code here, to run repeatedly:

//  int sensorValue = analogRead(SensorPin);
//
//  Serial.print("Sensor Value: ");
//  Serial.print(sensorValue);
//  //Serial.println();
//
//  float voltage = (sensorValue/1024.0) * 5.0;
//
//  Serial.print(", Volts: ");
//  Serial.print(voltage);
//  //Serial.println();
//
//  float temperature = (voltage - .5) * 100;
//
//  Serial.print(", degrees C: ");
//  Serial.print(temperature);
//  Serial.println();
  
  digitalWrite(LedPin1, HIGH);
}
