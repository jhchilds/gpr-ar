  #include <Arduino.h>
#define NOT_AN_INTERRUPT -1
#define setThresholdPin A0

volatile unsigned long tickCounts;
volatile boolean counterReady;
volatile int pulse;
const int maxTicks = 50;

// CHECK IF PINS ARE GONNA WORK

void encoderCounter(){
  counterReady = false;
//  maxTicks = analogRead(setThresholdPin) + 10;
  // Have to use a timer to count the rising edges on a pin
  // Following Gammon's example, I will start by resetting the control registers
  TCCR1A = 0;
  TCCR1B = 0;
  
  TCCR2A = 0;
  TCCR2B = 0;
  
  TIMSK2 = 0;
  
  // Furthermore we may want to handle the case of overflow, since the timer is a 8-bit register
  // So we set the Timer Overflow Interrupt Event of Timer 1
  
  // I want to generate an event every N ticks
  // I will set the Timer 1 to CTC (Clear Timer on Compare Match) mode
  TCCR1A = bit (WGM11);
  
  // And to enable the interrupt on match with OCA 
  TIMSK1 = bit (OCIE1A);
  
  // I can use the Output Compare to do that by settings its value to N
   OCR1A = maxTicks;
   
   // Set counter to zero
   TCNT1 = 0;
  
  // When the timer and the OC matches, a match event will be generated by triggering the TIMER1_COMPA_vect ISF below
  // Set the clock select to use an external clock source on Timer 1 pin and count rising edges
  TCCR1B = bit (CS10) | bit (CS11) | bit (CS12);
  // This event will turn a led on and off
}

// Interrupt Service Routine (ISR), Following Gammon's example
ISR (TIMER1_COMPA_vect)
{
  // Grab counter value
  unsigned int timer1CounterValue;
  timer1CounterValue = TCNT1;
  
  TCCR1A = 0;
  TCCR1B = 0;
  
  TIMSK1 = 0;
 
  tickCounts = timer1CounterValue; 
  counterReady = true;
}

void setup(){
  pinMode(8, OUTPUT);
  pinMode(13, OUTPUT);
  digitalWrite(8, LOW);
  digitalWrite(13, LOW);
//  Serial.begin(115200);
//  Serial.println("Tick counter");
}

void loop() {
  Serial.print("---");
  Serial.println(millis());
  Serial.println("Starting encoder counter");
  
  // Set peak to low
  digitalWrite(8, LOW);
  digitalWrite(13, LOW);
  
  // Following Gammon's example, not sure what he means by stopping Timer 0 interrupts from throwing the count out
  // But anyways, we stop the timer 0 while counting, storing its previous value
  byte oldTCCR0A = TCCR0A;
  byte oldTCCR0B = TCCR0B;
  TCCR0A = 0;
  TCCR0B = 0;
  
  encoderCounter();
  
  while (!counterReady){
     // TRY TO ADD TANGO BT CODE HERE
//     Serial.println(maxTicks);
    // Do nothig until counter is ready
//    Serial.println(tickCounts);
  }
  
  // TRY MOVING THIS INSIDE ISR OR PUT THE BT SIGNAL
//  Serial.println(tickCounts);
  digitalWrite(8, HIGH);
  digitalWrite(13, HIGH);
  
  // Restart Timer 0
  TCCR0A = oldTCCR0A;
  TCCR0B = oldTCCR0B;
  
  delay(1);
}
