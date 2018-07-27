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
    public static class ReadIt
    {
        //public static string GetValue()
        //{
        //    return "The value";
        //}

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
        //static string partitionId = "[PARTIION_ID]";
        static string ConnectionString = "";
        static string eventHubEntity = "";
        static string partitionId0 = "";
        static string partitionId1 = "";
        //static DateTime startingDateTimeUtc;

        static EventHubClient client;
        static EventHubConsumerGroup group;

        public static void GetAll(CancellationTokenSource cts)
        {
            //ServiceBusConnectionStringBuilder builder = new ServiceBusConnectionStringBuilder(ConnectionString);
            //builder.TransportType = TransportType.Amqp;

            MessagingFactory factory = MessagingFactory.CreateFromConnectionString(ConnectionString);

            client = factory.CreateEventHubClient(eventHubEntity);
            group = client.GetDefaultConsumerGroup();

            var tasks = new List<Task>();
            foreach (string partition in new List<string>(){ "0", "1"})
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
            }

            // Wait for all the PartitionReceivers to finish.
            Task.WaitAll(tasks.ToArray());

            client.Close();
            factory.Close();
        }

        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            DateTime startingDateTimeUtc = DateTime.Now;
            EventHubReceiver receiver0 = group.CreateReceiver(partition, startingDateTimeUtc);

            while (true)
            {
                if (ct.IsCancellationRequested) break;

                EventData data = receiver0.Receive();
                Debug.WriteLine("{0} {1} {2}", data.PartitionKey, data.EnqueuedTimeUtc.ToLocalTime(), Encoding.UTF8.GetString(data.GetBytes()));
            }
        }
    }
}
