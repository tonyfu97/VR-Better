using UnityEngine;
using System.IO.Ports;
//using System.Numerics;

public class CubeSerialController : MonoBehaviour
{
    private SerialPort serialPort;
    public string portName = "/dev/cu.usbmodem2101"; // Change this to the name of your serial port
    public int baudRate = 9600;
    private Transform cameraTransform;

    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
        cameraTransform = Camera.main.transform; // Get the main camera's transform
    }

    void Update()
    {
        // Get the current position of the cube relative to the camera
        Vector3 relativePosition = cameraTransform.InverseTransformPoint(transform.position);

        // Check if the cube is to the left or right of the camera
        string directionString = "";
        if (Vector3.Dot(relativePosition, cameraTransform.right) > 0)
        {
            directionString = "LEFT";
        }
        else
        {
            directionString = "RIGHT";
        }

        // Convert the position and direction to a string
        string positionString = "X:" + relativePosition.x + ",Y:" + relativePosition.y + ",Z:" + relativePosition.z;
        string messageString = positionString + "," + directionString;

        // Send the message to the serial port
        serialPort.WriteLine(messageString);
    }

    void OnDestroy()
    {
        serialPort.Close();
    }
}

