using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TxtWifiNtwrk.Text = WifiNetworkDefaultName;
            TxtWifiPwd.Text = WifiPasswordDefaultName;
        }

        bool sendedWifiNetworkToArduino = false;
        bool arduinoReceivedWifiNetwork = false;
        bool sendedWifiPasswordToArduino = false;
        bool arduinoReceivedWifiPassword = false;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string ardiuinoSerialPort = AutodetectArduinoPort();
            if (string.IsNullOrEmpty(ardiuinoSerialPort))
                return;

            SerialPort port = new SerialPort(ardiuinoSerialPort, 9600); //, Parity.None, 8, StopBits.One);
            port.Open();
            port.DataReceived += Port_DataReceived;

            sendedWifiNetworkToArduino = true;
            Console.WriteLine("Sending Wifi network.");
            port.WriteLine(TxtWifiNtwrk.Text);

            while(!arduinoReceivedWifiNetwork)
            {
                Console.WriteLine("Waiting for Received Wifi network answer.");
                Thread.Sleep(10);
            }

            sendedWifiPasswordToArduino = true;
            Console.WriteLine("Sending Wifi password.");
            port.WriteLine(TxtWifiPwd.Text);

        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            if (sendedWifiNetworkToArduino && !arduinoReceivedWifiNetwork)
            {
                Console.WriteLine("Received send Wifi network answer: " + port.ReadLine());
                arduinoReceivedWifiNetwork = true;
            }

            if (arduinoReceivedWifiNetwork && sendedWifiPasswordToArduino && !arduinoReceivedWifiPassword)
            {
                Console.WriteLine("Received send Wifi password answer: " + port.ReadLine());
                arduinoReceivedWifiPassword = true;
            }

            if(arduinoReceivedWifiNetwork && arduinoReceivedWifiPassword)
                Console.WriteLine("Received: " + port.ReadLine());
        }

        // https://stackoverflow.com/questions/3293889/how-to-auto-detect-arduino-com-port
        private string AutodetectArduinoPort()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("Arduino"))
                    {
                        return deviceId;
                    }
                }
            }
            catch (ManagementException e)
            {
                /* Do Nothing */
            }

            return null;
        }
    }
}
