using GrovePi;
using GrovePi.Sensors;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;

namespace MeasureIt
{
    public sealed partial class StartupTask : IBackgroundTask
    {
        const int MinDesiredTemperature = 15;
        const int MaxDesiredTemperature = 30;

        ILed redLed;
        ILed greenLed;
        IRotaryAngleSensor angleSensor;
        ITemperatureSensor temperatureSensor;

        private const string IotHubUri = "";
        private const string DeviceKey = "";


        private static DeviceClient _deviceClient;
        private static int _messageId = 1;

        //// Create a file StartupTask.Secrets.cs with a partial class for this class and add following declaration with the value for the deviceid and the connection string.
        //private const string DeviceId = "";
        //private readonly static string ConnectionString = "";


        //private static DeviceClient s_deviceClient;
        //private readonly static string s_connectionString = "";

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            redLed = DeviceFactory.Build.Led(Pin.DigitalPin2);
            greenLed = DeviceFactory.Build.Led(Pin.DigitalPin3);
            angleSensor = DeviceFactory.Build.RotaryAngleSensor(Pin.AnalogPin2);
            temperatureSensor = DeviceFactory.Build.TemperatureSensor(Pin.AnalogPin1);

            _deviceClient = DeviceClient.CreateFromConnectionString(ConnectionString, TransportType.Mqtt);

            //// Initial telemetry values
            //double minTemperature = 20;
            //double minHumidity = 60;
            //Random rand = new Random();

            //s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, TransportType.Mqtt);

            // Loop endlessly
            while (true)
            {
                try
                {
                    var angleValue = angleSensor.SensorValue();
                    var desiredTemperature = MinDesiredTemperature + ((MaxDesiredTemperature - MinDesiredTemperature) * angleValue / 1024);
                    var currentTemperature = temperatureSensor.TemperatureInCelsius();

                    System.Diagnostics.Debug.WriteLine("temperature is :" + currentTemperature + ", desired is: " + desiredTemperature);
                    if (currentTemperature < desiredTemperature)
                    {
                        redLed.ChangeState(SensorStatus.On);
                        greenLed.ChangeState(SensorStatus.Off);
                    }
                    else
                    {
                        redLed.ChangeState(SensorStatus.Off);
                        greenLed.ChangeState(SensorStatus.On);
                    }

                    var telemetryDataPoint = new
                    {
                        messageId = _messageId++,
                        //deviceId = DeviceId,
                        temperature = currentTemperature,
                    };
                    var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                    var message = new Message(System.Text.Encoding.ASCII.GetBytes(messageString));

                    _deviceClient.SendEventAsync(message).Wait();

                    //double currentTemperature = minTemperature + rand.NextDouble() * 15;
                    //double currentHumidity = minHumidity + rand.NextDouble() * 20;

                    //// Create JSON message
                    //var telemetryDataPoint = new
                    //{
                    //    temperature = currentTemperature,
                    //    humidity = currentHumidity
                    //};
                    //var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                    //var message = new Message(Encoding.ASCII.GetBytes(messageString));

                    //// Add a custom application property to the message.
                    //// An IoT hub can filter on these properties without access to the message body.
                    //message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

                    //// Send the tlemetry message
                    //s_deviceClient.SendEventAsync(message).Wait();
                    //Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                    Task.Delay(1000).Wait();

                }
                catch (Exception ex)
                {
                    // NOTE: There are frequent exceptions of the following:
                    // WinRT information: Unexpected number of bytes was transferred. Expected: '. Actual: '.
                    // This appears to be caused by the rapid frequency of writes to the GPIO
                    // These are being swallowed here/

                    // If you want to see the exceptions uncomment the following:
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
        }
    }
}
