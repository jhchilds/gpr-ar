#include <Arduino.h>
#include <SoftwareSerial.h>

#define rxPin 2
#define txPin 3
#define counterSignalPin A0

SoftwareSerial mySerial(rxPin, txPin);
int lastState = 0;
int val = 0;
bool isPaired = false;
bool firstSend = true;

void setup(){
  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, LOW);

  //Set up Tx/Rx pins
  pinMode(rxPin, INPUT);
  pinMode(txPin, OUTPUT);
  
  mySerial.begin(9600);
}

void loop() {
  val = analogRead(counterSignalPin);
  
  // wait for a handshake signal to either start or end
  int i = 0;
  char state[32] = {0};
  if (mySerial.available() > 0) {
    do {
      state[i++] = mySerial.read();
    } while (mySerial.available() > 0 );
  }
  
  if (state[0] == 'H') {
    isPaired = !isPaired;
  }
  
  if (val > 500) {
    digitalWrite(LED_BUILTIN, HIGH);
    mySerial.println('X');
    firstSend = false;
  } else {
    digitalWrite(LED_BUILTIN, LOW);
    firstSend = true;
  }
}
