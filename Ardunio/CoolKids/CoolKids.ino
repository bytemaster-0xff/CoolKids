#include <Wire.h>

int on;

void setup()
{
	Serial.begin(115200);
	Serial.println("LED Manager Startup\r\n");


//	pinMode(2, OUTPUT);

	Wire.begin(0x30);
	Wire.onReceive(receiveEvent);
	Wire.onRequest(requestEvent);

	Serial.println("Online accepting requests.\r\n");
}

void receiveEvent(int howMany) {
	

	for (int idx = 0; idx < howMany; ++idx)
	{
		byte ch = Wire.read();
	//	Serial.write(ch);
	}

	if (on == 1)
		digitalWrite(2, LOW);
	else
		digitalWrite(2, HIGH);
}

void requestEvent() {
	Wire.write(0);
}

void loop()
{
	delay(1000);
	Serial.write("X");
	/*digitalWrite(2, HIGH);
	digitalWrite(2, LOW);
	delay(1000);*/

  /* add main program code here */

}
