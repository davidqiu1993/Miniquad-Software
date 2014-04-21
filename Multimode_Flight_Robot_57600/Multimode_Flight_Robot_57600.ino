#include <Wire.h>
#include <Miniquad.h>

Miniquad copter;

int state = 0;           // 0: off, 1: on
float p = 1.0f;
float i = 0.0f;
float d = 0.0f;
int thave = 0;
int th[4] = { 0, 0, 0, 0 };
char buf[256];

void setup()
{
	Wire.begin();
	copter.Initialize();
	Serial.begin(57600);
}

void loop()
{
	for (int lc = 0; lc < 20; ++lc)
	{
		ctrlLogic();
	}
	commLogic(); // feedback + command
}


void ctrlLogic()
{
	copter.RefreshDmpData();

	if (state) refreshThrottles();
	else stopThrottles();
	copter.PropellerSetAllSpeeds(
		th[0],
		th[1],
		th[2],
		th[3]);
}

void commLogic()
{
	Serial.readall(buf);
	// TODO !!!
}


void stopThrottles()
{
	th[0] = 0;
	th[1] = 0;
	th[2] = 0;
	th[3] = 0;
}

void refreshThrottles()
{
	th[0] = 0;
	th[2] = 0;

	int tht = thave * 2;
	th[1] = (tht*(90 - copter.GetYawPitchRoll().getPitch())) / 180;
	th[3] = (tht*(90 + copter.GetYawPitchRoll().getPitch())) / 180;
	if (th[1] > 255) th[3] = (255 * (90 + copter.GetYawPitchRoll().getPitch())) / 180;
	if (th[3] > 255) th[1] = (255 * (90 - copter.GetYawPitchRoll().getPitch())) / 180;
}

char readChar()
{
	return Serial.read();
}

int readInt()
{
	int data;
	char* ptr = (char*)((void*)(&data));
	for (int i = 3; i >= 0; --i)
		*(ptr + i) = Serial.read();
	return data;
}

int readFloat()
{
	int i;
	float data;
	char* ptr = (char*)((void*)(&data));
	for (i = 3; i >= 0; --i)
		*(ptr + i) = Serial.read();
	return data;
}



