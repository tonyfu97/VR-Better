#include <Arduino_LSM9DS1.h>
#include <ArduinoBLE.h>

const int motor1Pin = 7;
const int motor2Pin = 8;
const int motor3Pin = 9;
const int motor4Pin = 10;

void setup() {
  Serial.begin(115200);
  delay(2000);
//  while(!Serial);

  // Start IMU
  if (!IMU.begin()) {
    Serial.println("Failed to initialized IMU!");
    while (1);
  }

  IMU.setContinuousMode();
  const float acceleration_sample_rate = IMU.accelerationSampleRate();
  const float gyroscope_sample_rate = IMU.gyroscopeSampleRate();

  pinMode(motor1Pin, OUTPUT);
  pinMode(motor2Pin, OUTPUT);
  pinMode(motor3Pin, OUTPUT);
  pinMode(motor4Pin, OUTPUT);
}

void loop() {
  static float gx, gy, gz;

  if (IMU.gyroscopeAvailable()) {
    IMU.readGyroscope(gx, gy, gz);
  }

  if (Serial.available()) {
    String input_str = Serial.readStringUntil('\n');
    int idx = 0;
    int motor_intensities[4] = {0, 0, 0, 0};

    // Parse the comma-separated angles string
    for (int i = 0; i < input_str.length(); i++) {
      if (input_str[i] == ',') {
        idx++;
      } else {
        motor_intensities[idx] = motor_intensities[idx] * 10 + input_str[i] - '0';
      }
    }

    // output to vibration motors
    for (int i = 0; i < 4; i++) {
      int motor_intensity = motor_intensities[i];
      analogWrite(motor1Pin + i, motor_intensity);
    }
  }
  
  delay(20);

  Serial.print(gx);
  Serial.print(",");
  Serial.print(gy);
  Serial.print(",");
  Serial.println(gz);
}
