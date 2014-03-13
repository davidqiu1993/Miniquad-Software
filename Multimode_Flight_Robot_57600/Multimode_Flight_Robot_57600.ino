#include <Wire.h>
#include <MiniquadZero.h>


// The global instance for MiniquadZero
MiniquadZero copter;


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
