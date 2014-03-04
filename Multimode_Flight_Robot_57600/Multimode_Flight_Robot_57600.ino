#include <Wire.h>
#include <MiniquadZero.h>

void setup()
{
	init();
}

void loop()
{
	for (int i = 0; i<20; ++i) ctrlLogic();
	commLogic();
}
