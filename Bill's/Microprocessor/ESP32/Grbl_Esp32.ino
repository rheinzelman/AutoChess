//================================================================================
//
//================================================================================
#include "src/Grbl.h"
#include "FastLED.h"
//================================================================================
//
//================================================================================
TaskHandle_t Board_S;
TaskHandle_t corexy;
//================================================================================
//
//================================================================================
#define NUM_LEDS 90
#define LED_PIN 15
//================================================================================
//
//================================================================================
CRGB leds[NUM_LEDS];
//================================================================================
//
//================================================================================
const int dataPin  = 25;   /* Q7 */
const int clockPin = 33;  /* CP */
const int latchPin = 32;  /* PL */
//================================================================================
//
//================================================================================
const int numBits = 65; 
int boardArray[numBits - 1];
byte arrayIndex = 0;
//================================================================================
//
//================================================================================
int count = 0;
int count1 =0;
int location;
//================================================================================
//
//================================================================================
void setup() 
{ 
  grbl_init();
  Serial.begin(115200); 
  pinMode(dataPin, INPUT);
  pinMode(clockPin, OUTPUT);
  pinMode(latchPin, OUTPUT);
  FastLED.addLeds<WS2812B, LED_PIN, RGB>(leds,  NUM_LEDS);
  FastLED.setBrightness(10);
  xTaskCreatePinnedToCore(corexy_S,"CoreXY",10000,NULL,1,&corexy,1);
  xTaskCreatePinnedToCore(Board_Sensors,"Board Sensors",40000,NULL,2,&Board_S,0);     
         
}
////================================================================================
////
////================================================================================
void corexy_S( void * pvParameters )
{
  Serial.print("Task1 running on core ");
  Serial.println(xPortGetCoreID());
  run_once();
  delay(10000);
  for(;;)
  {
    delay(1000);
  }
}
////================================================================================
////
////================================================================================
void Board_Sensors( void * pvParameters )
{
  Serial.print("Task2 running on core ");
  Serial.println(xPortGetCoreID());
  for(;;)
  {
    digitalWrite(latchPin, LOW);
    digitalWrite(latchPin, HIGH);
    
    for (int i = 0; i < numBits; i++) 
    {
      int bit = digitalRead(dataPin);
      if (bit == HIGH) 
        {
          Serial.print("0");
          count ++;
        } 
      else 
        {
          Serial.print("1");
          count++;
        }
      
      digitalWrite(clockPin, HIGH);
      digitalWrite(clockPin, LOW);
      
      if(count == numBits)
      {
        Serial.println();
        count = 0;
      }
   }
   delay(1000);
   
  }
}

//================================================================================
//
//================================================================================
void loop() 
  {
    
if(Serial.available()>0)
{
      location = Serial.read();
     switch(location)
      {
      case 'a' :
        leds[1] = CRGB(0,0,128);
        FastLED.show();
        delay(10);
      break;
      case 'b' :
        leds[2] = CRGB(0,0,128);
        FastLED.show();
        delay(10);
      break;
      }
}
    delay(1000);
    //run_once();
  //   for(int dot=(NUM_LEDS-1) ; dot >=0 ; dot--)
//    { 
//      leds[dot] = CRGB(0,0,128);
//       FastLED.show();
//      delay(10);
//    }
//    for(int dot = 0;dot < NUM_LEDS; dot++)
//    {
//      leds[dot] = CRGB(128,0,0);
//      FastLED.show();
//      delay(10);
//    }
//     digitalWrite(latchPin, LOW);
//    digitalWrite(latchPin, HIGH);
//    
//    for (int i = 0; i < numBits; i++) 
//    {
//      int bit = digitalRead(dataPin);
//      if (bit == HIGH) 
//        {
//          Serial.print("0");
//          count ++;
//        } 
//      else 
//        {
//          Serial.print("1");
//          count++;
//        }
//      
//      digitalWrite(clockPin, HIGH);
//      digitalWrite(clockPin, LOW);
//      
//      if(count == numBits)
//      {
//        Serial.println();
//        count = 0;
//      }
//   }
//   delay(1000);
  }
