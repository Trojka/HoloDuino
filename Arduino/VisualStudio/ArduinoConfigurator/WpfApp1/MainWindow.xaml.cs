using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string ardiuinoSerialPort = AutodetectArduinoPort();
            if (string.IsNullOrEmpty(ardiuinoSerialPort))
                return;

            SerialPort port = new SerialPort(ardiuinoSerialPort, 9600); //, Parity.None, 8, StopBits.One);
            port.Open();
            port.DataReceived += Port_DataReceived;

            port.WriteLine(TxtWifiNtwrk.Text);
            port.WriteLine(TxtWifiPwd.Text);
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort spL = (SerialPort)sender;
            //const int bufSize = 12;
            //Byte[] buf = new Byte[bufSize];
            //Console.WriteLine("DATA RECEIVED!");
            //Console.WriteLine(spL.Read(buf, 0, bufSize));
            Console.WriteLine(spL.ReadLine());
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
