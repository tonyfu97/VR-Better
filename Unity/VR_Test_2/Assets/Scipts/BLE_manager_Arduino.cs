using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using TMPro;


public class BLE_manager_Arduino : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    private BluetoothHelper bluetoothHelper;
    private float timer;
    void Start()
    {
        timer = 0;
        try
        {
            UpdateDebugText("HI");

            BluetoothHelper.BLE = true;  //use Bluetooth Low Energy Technology
            bluetoothHelper = BluetoothHelper.GetInstance();
            bluetoothHelper.OnConnected += (helper) => {
                List<BluetoothHelperService> services = helper.getGattServices();
                foreach (BluetoothHelperService s in services)
                {
                    UpdateDebugText("Service : " + s.getName());
                    foreach (BluetoothHelperCharacteristic item in s.getCharacteristics())
                    {
                        Debug.Log(item.getName());
                    }
                }

                UpdateDebugText("Connected");
                BluetoothHelperCharacteristic c = new BluetoothHelperCharacteristic("19B10001-E8F2-537E-4F6C-D104768A1214");
                c.setService("19B10000-E8F2-537E-4F6C-D104768A1214");
                bluetoothHelper.Subscribe(c);
                //sendData();
            };
            bluetoothHelper.OnConnectionFailed += (helper) => {
                UpdateDebugText("Connection failed");
            };
            bluetoothHelper.OnScanEnded += OnScanEnded;
            bluetoothHelper.OnServiceNotFound += (helper, serviceName) =>
            {
                UpdateDebugText(serviceName);
            };
            bluetoothHelper.OnCharacteristicNotFound += (helper, serviceName, characteristicName) =>
            {
                UpdateDebugText(characteristicName);
            };
            bluetoothHelper.OnCharacteristicChanged += (helper, value, characteristic) =>
            {
                UpdateDebugText(characteristic.getName());
                Debug.Log(value[0]);
            };

            bluetoothHelper.ScanNearbyDevices();

        }
        catch (Exception ex)
        {
            UpdateDebugText("Debug: " + ex.ToString());
        }
    }

    private void OnScanEnded(BluetoothHelper helper, LinkedList<BluetoothDevice> devices)
    {
        UpdateDebugText("Found " + devices.Count);
        if (devices.Count == 0)
        {
            bluetoothHelper.ScanNearbyDevices();
            return;
        }

        foreach (var d in devices)
        {
            UpdateDebugText(d.DeviceName);
        }

        try
        {
            bluetoothHelper.setDeviceName("ArduinoNanoVibrationMotor"); // Replace "HC-08" with "MyArduinoNano"
            bluetoothHelper.Connect();
            UpdateDebugText("Connecting");
        }
        catch (Exception ex)
        {
            bluetoothHelper.ScanNearbyDevices();
            UpdateDebugText(ex.Message);
        }

    }

    void OnDestroy()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
    }

    void Update()
    {
        if (bluetoothHelper == null)
            return;
        if (!bluetoothHelper.isConnected())
            return;
        timer += Time.deltaTime;

        if (timer < 5)
            return;
        timer = 0;
        //sendData();
    }

    public void SendMotorCommand(string motorCommand)
    {
        UpdateDebugText(motorCommand);
        sendData(motorCommand);
    }

    void sendData(string motorCommand)
    {
        UpdateDebugText("Sending");
        BluetoothHelperCharacteristic ch = new BluetoothHelperCharacteristic("19B10001-E8F2-537E-4F6C-D104768A1214");
        ch.setService("19B10000-E8F2-537E-4F6C-D104768A1214");
        // Replace "1234" with the motor intensities you want to send as a comma-separated string, e.g., "100,200,50,150"
        bluetoothHelper.WriteCharacteristic(ch, motorCommand);
    }

    void read()
    {
        BluetoothHelperCharacteristic ch = new BluetoothHelperCharacteristic("2A24");
        ch.setService("180A");//this line is mandatory!!!
        bluetoothHelper.ReadCharacteristic(ch);
    }

    void UpdateDebugText(string message)
    {
        if (debugText != null)
        {
            debugText.text = message;
            Debug.Log(message);
        }
    }
}
