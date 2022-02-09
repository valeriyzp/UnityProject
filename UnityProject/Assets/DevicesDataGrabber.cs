using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;

public class DevicesDataGrabber : MonoBehaviour
{
    DateTime lastUpdateTime;

    private InputDevice LeftController;
    //private InputDevice RightController;

    private string directoryPath = "C:\\Users\\Admin\\Desktop\\";
    private string screenshotPath;
    private string csvExportFilePath;

    private Dictionary<string, string> devicesData = new Dictionary<string, string>();

    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, devices);

        foreach (var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }

        if (devices.Count > 0)
        {
            LeftController = devices[0];
        }

        devicesData["date"] = "";
        devicesData["left_controller_primary2daxis_x"] = "";
        devicesData["left_controller_primary2daxis_y"] = "";
        devicesData["left_controller_primary2daxistouch"] = "";
        devicesData["left_controller_primary2daxisclick"] = "";

        lastUpdateTime = DateTime.Now;

        directoryPath += lastUpdateTime.ToString("yyyyMMddHHmmssfff");
        csvExportFilePath = directoryPath + "\\" + "data.csv";
        screenshotPath = directoryPath + "\\" + "s_";

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        using (StreamWriter sw = File.CreateText(csvExportFilePath))
        {
            string headerString = "";
            foreach (var str in devicesData.Keys)
            {
                headerString += str + ";";
            }
            headerString = headerString.TrimEnd(';');

            sw.WriteLine(headerString);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DateTime updateTime = DateTime.Now;

        if ((updateTime - lastUpdateTime).TotalMilliseconds >= 100)
        {
            string currentTime = updateTime.ToString("yyyyMMddHHmmssfff");

            ScreenCapture.CaptureScreenshot(screenshotPath + currentTime + ".png");
            Debug.Log("ScreenshotCaptured");

            LeftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2dAxisValue);
            Debug.Log("Primart Touchpad " + primary2dAxisValue);

            LeftController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool primary2daxisclick);
            Debug.Log("Primart Touchpad Click " + primary2daxisclick);

            LeftController.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool primary2daxistouch);
            Debug.Log("Primart Touchpad Touch " + primary2daxistouch);

            ControllerData leftControllerData = new ControllerData();
            leftControllerData.date = updateTime;
            leftControllerData.primary2daxis_x = primary2dAxisValue.x;
            leftControllerData.primary2daxis_y = primary2dAxisValue.y;
            leftControllerData.primary2daxisclick = primary2daxisclick;
            leftControllerData.primary2daxistouch = primary2daxistouch;

            devicesData["date"] = updateTime.ToString("yyyyMMddHHmmssfff");
            devicesData["left_controller_primary2daxis_x"] = leftControllerData.primary2daxis_x.ToString();
            devicesData["left_controller_primary2daxis_y"] = leftControllerData.primary2daxis_y.ToString();
            devicesData["left_controller_primary2daxistouch"] = leftControllerData.primary2daxisclick.ToString();
            devicesData["left_controller_primary2daxisclick"] = leftControllerData.primary2daxistouch.ToString();

            using (StreamWriter sw = File.AppendText(csvExportFilePath))
            {
                string dataString = "";
                foreach (var key in devicesData.Keys)
                {
                    dataString += devicesData[key] + ";";
                }
                dataString = dataString.TrimEnd(';');

                sw.WriteLine(dataString);
            }
        }
    }

    [Serializable]
    public class ControllerData
    {
        [SerializeField]
        public DateTime date;
        [SerializeField]
        public double primary2daxis_x;
        [SerializeField]
        public double primary2daxis_y;
        [SerializeField]
        public bool primary2daxisclick;
        [SerializeField]
        public bool primary2daxistouch;
    }
}
