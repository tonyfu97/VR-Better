import serial
import time
from datetime import datetime

def control_motor(intensities):
    intensity_str = ','.join(str(intensity) for intensity in intensities)
    print(intensity_str)
    ser.write(intensity_str.encode('utf-8') + b'\n')

def get_imu():
    ser.reset_input_buffer()
    time.sleep(0.01)
    imu = ser.readline().strip().decode('utf-8')
    if len(imu.split(',')) == 3:
        return imu
    else:
        return None

# Serial port settings
serial_port = '/dev/cu.usbmodem2101'
baud_rate = 115200

# Connect to the Arduino
ser = serial.Serial(serial_port, baud_rate, timeout=1)
time.sleep(2)  # Wait for the connection to establish

# Format log file name
current_time = datetime.now().strftime("%Y-%m-%d_%H-%M-%S")
log_filename = f"logs/log{current_time}.txt"

motor_map = {'w': 0, 'a': 1, 's': 2, 'd': 3}
intensities = [0, 0, 0, 0]

print("Enter 'w', 'a', 's', 'd' followed by an intensity value (max 255) to control the motors. Press 'q' to quit.")

while True:
    user_input = input("Enter command: ")

    if user_input.lower() == 'q':
        break

    try:
        key, intensity = user_input[0], int(user_input[1:])
        if key in motor_map:
            selected_motor = motor_map[key]
            intensities[selected_motor] = intensity
            control_motor(intensities)
            print("Motor", selected_motor, "intensity set to", intensities[selected_motor])
            
            start_time = time.time()
            with open(log_filename, 'a') as log_file:
                log_file.write(user_input + "\n")
                while time.time() - start_time < 1.0:
                    imu = get_imu()
                    if imu is not None:
                        log_file.write(f"{imu}\n")
                        print(imu)
            
            # Turn off after some period
            intensities = [0, 0, 0, 0]
            control_motor(intensities)
        else:
            print("Invalid command. Please enter 'w', 'a', 's', 'd' followed by an intensity value (max 255).")
    except ValueError:
        print("Error: Invalid input. Please enter 'w', 'a', 's', 'd' followed by an intensity value (max 255).")

# Turn off motors
control_motor([0, 0, 0, 0])
ser.close()


# w = right
# a = top left?
# s = left
# d = top right?


