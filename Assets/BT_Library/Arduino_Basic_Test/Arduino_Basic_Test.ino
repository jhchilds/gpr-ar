#include <Printers.h>
#include <XBee.h>


void setup () {
  Serial.begin(9600);
}


void loop() {
  if(Serial.available())
    Serial.print((char)Serial.read());
}
