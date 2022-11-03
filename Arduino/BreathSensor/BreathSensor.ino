int LEDc = 2;

/* Definition of constants */
#define SAMPLING_DELAY_MS (5)
#define SENSOR_NAME ("NithBS_5010DP-0.2.0")

void setup() {
  // initialize serial communication at 9600 bits per second:
  Serial.begin(115200);
  pinMode(LEDc, OUTPUT);
  digitalWrite(LEDc, HIGH);
}

void loop() {
  
  int sensorValue = analogRead(A0); // read the input on analog pin 0
  float voltage = sensorValue / 3;
  
  /* Print output line */
  /* =============================================== */
  
  /* Line start character */
  Serial.print("$");
  
  /* Sensor name and version */
  Serial.print(SENSOR_NAME);
  Serial.print("|");

  /* Status line */
  Serial.print("OPR");
  Serial.print("|");
  
  /* Values */
  Serial.print("press=");
  Serial.println(voltage, DEC);
  delay(SAMPLING_DELAY_MS);
}