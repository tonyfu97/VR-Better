import serial
import time

def control_motor(intensities):
    intensity_str = ','.join(str(intensity) for intensity in intensities)
    ser.write(intensity_str.encode('utf-8') + b'\n')

def get_imu():
    imu = ser.readline().strip().decode('utf-8')
    if len(imu.split(',')) == 3:
        return imu
    else:
        return None

# Serial port settings
serial_port = '/dev/cu.usbmodem2101'
baud_rate = 115200

output_file = "log.txt"

# Connect to the Arduino
ser = serial.Serial(serial_port, baud_rate, timeout=1)
time.sleep(2)  # Wait for the connection to establish

while True:
    user_input = input("Enter comma-separated angle values for the servo motors or 'q' to quit: ")

    if user_input.lower() == 'q':
        break

    try:
        intensities = [int(intensity) for intensity in user_input.split(',')]
        control_motor(intensities)
        
        with open(output_file, 'w') as f:
            f.write(f"Motor intensities: {intensities}\n")
            ser.reset_input_buffer()
            for i in range(30):
                imu = get_imu()
                if imu is not None:
                    f.write(imu + "\n")
                    print(imu)

        # Turn off motor
        control_motor([0,0,0,0])

    except ValueError:
        print("Error: Invalid input. Please enter comma-separated angle values or 'q' to quit.")

ser.close()
