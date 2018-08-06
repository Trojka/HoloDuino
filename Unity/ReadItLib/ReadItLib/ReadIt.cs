using Microsoft.Azure.Devices;
using ppatierno.AzureSBLite;
using ppatierno.AzureSBLite.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadItLib
{
    public static partial class ReadIt
    {
        private static bool _stopProcessing;
        private static int _maxRead = 20;

        public static async Task<int> StartProcessing(Action<string> action)
        {
            _stopProcessing = false;

            int i = 0;

            Random rnd = new Random();

            while (!_stopProcessing && i < _maxRead)
            {
                int temp = rnd.Next(100);
                action(temp.ToString());
                i++;

                await Task.Delay(1000);
            }

            return 0;
        }

        public static void StopProcessing()
        {
            _stopProcessing = true;
        }

        //static string ConnectionString = "Endpoint=[EVENT_HUB_COMPATIBLE_ENDPOINT];SharedAccessKeyName=[IOT_HUB_POLICY_NAME];SharedAccessKey=[IOT_HUB_POLICY_KEY]";
        //static string eventHubEntity = "[EVENT_HUB_COMPATIBLE_NAME]";
        //static List<string> PartitionIds = new List<string>() { "[PARTIION_ID_1]", ... };
        //static DateTime startingDateTimeUtc;

        static MessagingFactory factory;
        static EventHubClient client;
        static EventHubConsumerGroup group;

        static List<EventHubReceiver> receiverList;

        static CancellationTokenSource cts;

        public static async Task Start(Action<string> processEvent)
        {
            cts = new CancellationTokenSource();
            receiverList = new List<EventHubReceiver>();
            await GetAll(processEvent, cts);
        }

        public static void Stop()
        {
            cts.Cancel();

            receiverList.ForEach(r => r.Close());
            client.Close();
            factory.Close();
        }


        public static async Task GetAll(Action<string> processEvent, CancellationTokenSource cts)
        {
            //ServiceBusConnectionStringBuilder builder = new ServiceBusConnectionStringBuilder(ConnectionString);
            //builder.TransportType = TransportType.Amqp;

            Action<string> eventAction = processEvent;

            factory = MessagingFactory.CreateFromConnectionString(ConnectionString);

            client = factory.CreateEventHubClient(eventHubEntity);
            group = client.GetDefaultConsumerGroup();

            var tasks = new List<Task>();
            foreach (string partition in PartitionIds)
            {
                tasks.Add(Task.Run(() => ReceiveMessagesFromDeviceAsync(partition, eventAction, cts.Token)));
            }

            // Wait for all the PartitionReceivers to finish.
            await Task.WhenAll(tasks.ToArray());
        }

        private static async Task ReceiveMessagesFromDeviceAsync(string partition, Action<string> processEvent, CancellationToken ct)
        {
            DateTime startingDateTimeUtc = DateTime.Now;
            EventHubReceiver receiver0 = group.CreateReceiver(partition, startingDateTimeUtc);

            receiverList.Add(receiver0);

            while (true)
            {
                if (ct.IsCancellationRequested) break;

                if(!receiver0.IsClosed)
                {
                    EventData data = receiver0.Receive();

                    string theEvent = Encoding.UTF8.GetString(data.GetBytes());

                    processEvent(theEvent);
                    Debug.WriteLine("{0} {1} {2}", data.PartitionKey, data.EnqueuedTimeUtc.ToLocalTime(), theEvent);
                }

                await Task.Delay(2000);
            }
        }


        public static async Task updateDeviceIdsComboBoxes(bool runIfNullOrEmpty = true)
        {
            //if (!String.IsNullOrEmpty(activeIoTHubConnectionString) || runIfNullOrEmpty)
            {
                List<string> deviceIdsForEvent = new List<string>();
                List<string> deviceIdsForC2DMessage = new List<string>();
                List<string> deviceIdsForDeviceMethod = new List<string>();
                RegistryManager registryManager = RegistryManager.CreateFromConnectionString(AzureIotConnectionString);

                var devices = await registryManager.GetDevicesAsync(1000);
                foreach (var device in devices)
                {
                    deviceIdsForEvent.Add(device.Id);
                    deviceIdsForC2DMessage.Add(device.Id);
                    deviceIdsForDeviceMethod.Add(device.Id);
                }
                await registryManager.CloseAsync();
            }
        }
    }
}
