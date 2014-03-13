#include <Wire.h>
#include <Miniquad.h>


// The global instance for MiniquadZero
Miniquad copter;


void setup()
{
	// Wire initialization
	Wire.begin();

	// Miniquad Zero initialization
	copter.Initialize();

	// Serial initialization
	Serial.begin(57600);
}


void loop()
{
	for (int i = 0; i<20; ++i) ctrlLogic();
	commLogic();
}
