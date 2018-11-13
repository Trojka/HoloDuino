using ArduinoDeviceManagement;
using ArduinoDeviceManagement.Arduino;
using ArduinoDeviceManagement.Azure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var comPorts = ArduinoComm.GetComPorts();
            CmbComPorts.ItemsSource = comPorts;
            CmbComPorts.SelectedValuePath = "Id";

            var arduinoPortId = ArduinoComm.AutodetectArduinoPortId();
            CmbComPorts.SelectedValue = arduinoPortId;
        }

        bool sendedWifiNetworkToArduino = false;
        bool arduinoReceivedWifiNetwork = false;
        bool sendedWifiPasswordToArduino = false;
        bool arduinoReceivedWifiPassword = false;

        private async void BtnRegisterClick(object sender, RoutedEventArgs e)
        {
            var ardiuinoSerialPort = CmbComPorts.SelectedValue as string;
            if (string.IsNullOrEmpty(ardiuinoSerialPort))
                return;

            SerialPort port = new SerialPort(ardiuinoSerialPort, 9600); //, Parity.None, 8, StopBits.One);
            port.Open();

            var deviceIdReceiver = new DeviceIdReader();
            var deviceId = await deviceIdReceiver.GetData(port);
            Debug.WriteLine(deviceId);

            var deviceDescriptionReceiver = new DeviceDescriptionReader();
            var deviceDescription = await deviceDescriptionReceiver.GetData(port);
            Debug.WriteLine(deviceDescription);

            var wifiSSIDWriter = new WifiSSIDWriter();
            wifiSSIDWriter.SetData(port, TxtWifiNtwrk.Text);

            var wifiPWDWriter = new WifiPWDWriter();
            wifiPWDWriter.SetData(port, TxtWifiPwd.Text);


            byte[] buffer = new byte[80];
            while (true)
            {
                await port.BaseStream.ReadAsync(buffer, 0, 80);
                string receivedData = port.Encoding.GetString(buffer);

                Debug.WriteLine(receivedData);
            }



            port.Close();

            //var manager = new DeviceRegistrar(IoTHubConnectionString);
            //manager.RegisterDevice(deviceId, deviceDescription);

            return;

        }
    }
}
