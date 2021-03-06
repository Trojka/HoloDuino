﻿using Amqp;
using ReadItLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ReadItApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CancellationTokenSource cts;

        public MainPage()
        {
            this.InitializeComponent();

        }

        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            ReadItLib.ReadIt.Stop();
        }

        bool toggle = true;
        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
                toggle = !toggle;
                ActivateIt.ExecLedOn(DeviceId, toggle);



                //ReadItLib.ReadIt.Start(this.ShowEvent);

                //ReadItLib.ReadIt.updateDeviceIdsComboBoxes(this.ShowDevices);

                //ReadItLib.ReadIt.GetDevices();
            }

        private async void ShowEvent (string theEvent) {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                TheEventDisplay.Text = theEvent;
            });
        }

        private async void ShowDevices (bool ready, List<string> deviceIds)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                DeviceListDisplay.Text = string.Join(",", deviceIds);
            });
        }
    }
}
