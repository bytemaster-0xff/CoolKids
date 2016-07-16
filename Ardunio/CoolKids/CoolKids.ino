#include <Wire.h>
#include "dht11.h"


#define RELAY1_PIN1 2
#define RELAY1_PIN2 3
#define RELAY1_PIN3 4
#define RELAY1_PIN4 5
#define DHT11PIN 6


int on;



//dht11 DHT11;


//Celsius to Fahrenheit conversion
double Fahrenheit(double celsius)
{
	return 1.8 * celsius + 32;
}

void receiveEvent(int howMany) {
	Serial.write('x');


	for (int idx = 0; idx < howMany; ++idx)
	{
		byte ch = Wire.read();
		parseCommandString(ch);
	}
}


void requestEvent() {
	// ch = (DHT11.temperature >> 8) && 0xFF;
	//Wire.write(ch);
	//ch = DHT11.temperature && 0xFF;
	//Wire.write(DHT11.temperature && 0xFF);

	//	Wire.write(DHT11.temperature);
	Wire.write(1234);
	//Serial.write("f");
}



#define BUFFER_SIZE 10

char strBuffer[BUFFER_SIZE];
byte strBufferLen;
char cmd[BUFFER_SIZE];
byte cmdLen;


int argCount = -1;
char args[6][BUFFER_SIZE];
byte argLen[6];


void resetParser() {
	argCount = -1;

	strBufferLen = 0;
	cmdLen = 0;

	memset(cmd, 0x00, BUFFER_SIZE);
	memset(strBuffer, 0x00, BUFFER_SIZE);

	for (int idx = 0; idx < 12; ++idx) {
		memset(args[idx], 0x00, BUFFER_SIZE);
		argLen[idx] = 0;
	}
}

byte messageBuffer[16];

void processCommand() {
	Serial.write("Process Command: ");
	Serial.write(cmd, cmdLen);
	Serial.write("\r\n");

	if (strcmp(cmd, "r") == 0) {
		int relay = atoi(args[0]);
		int state = atoi(args[1]);

		Serial.write(args[0]);
		Serial.write(args[1]);
	
		if (state == 1)
			digitalWrite(relay, HIGH);
		else
			digitalWrite(relay, LOW);
	}
	else if (strcmp(cmd, "t") == 0) {

	}
	
	resetParser();
}

void handleDelimiter() {
	if (argCount == -1) {
		memcpy(cmd, strBuffer, strBufferLen);
		cmdLen = strBufferLen;
	}
	else {
		memcpy(args[argCount], strBuffer, strBufferLen);
		argLen[argCount] = strBufferLen;
	}

	argCount++;

	strBufferLen = 0;
	memset(strBuffer, 0x00, 12);
}

void parseCommandString(uint8_t ch) {
	switch (ch) {
	case ' ':
		handleDelimiter();
		break;
	case ';':
		handleDelimiter();
		processCommand();
		break;
	case '\r': /*NOP*/ break;
	case '\n': /*NOP*/ break;
	default:
		strBuffer[strBufferLen++] = ch;
	}
}


byte _lastHasMotion = 0;

void setup()
{
	Wire.begin(0x60);
	Wire.onReceive(receiveEvent);
	Wire.onRequest(requestEvent);

	Serial.begin(115200);
	Serial.println("LED Manager Startup\r\n");


	pinMode(RELAY1_PIN1, OUTPUT);
	pinMode(RELAY1_PIN2, OUTPUT);
	pinMode(RELAY1_PIN3, OUTPUT);
	pinMode(RELAY1_PIN4, OUTPUT);

	digitalWrite(RELAY1_PIN1, HIGH);
	digitalWrite(RELAY1_PIN2, HIGH);
	digitalWrite(RELAY1_PIN3, HIGH);
	digitalWrite(RELAY1_PIN4, HIGH);

	resetParser();

	Serial.println("Online accepting requests.\r\n");
}



// main loop
void loop()
{
	delay(1);
}
