//using Microsoft.Azure.Devices;
using ppatierno.AzureSBLite;
using ppatierno.AzureSBLite.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadItLib
{
    public static partial class ReadIt
    {
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
            Action<string> eventAction = processEvent;

            factory = MessagingFactory.CreateFromConnectionString(ConnectionString);

            client = factory.CreateEventHubClient(eventHubEntity);
            group = client.GetDefaultConsumerGroup();

            var tasks = new List<Task>();
            foreach (string partition in PartitionIds)
            {
                tasks.Add(Task.Run(() => ReceiveMessagesFromDeviceAsync(partition, eventAction, cts.Token)));
            }

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
                    Debug.WriteLine($"{data.PartitionKey} {data.EnqueuedTimeUtc.ToLocalTime()} {theEvent}");
                }

                await Task.Delay(2000);
            }
        }

        //public static async Task updateDeviceIdsComboBoxes(Action<bool, List<string>> processList)
        //{
        //    List<string> deviceIds = new List<string>();
        //    RegistryManager registryManager = RegistryManager.CreateFromConnectionString(AzureIotConnectionString);

        //    var query = registryManager.CreateQuery("SELECT * FROM devices", 100);
        //    while (query.HasMoreResults)
        //    {
        //        var page = await query.GetNextAsTwinAsync();
        //        foreach (var twin in page)
        //        {
        //            deviceIds.Add(twin.DeviceId);
        //            Debug.WriteLine($"Twin: {twin.ToString()}");
        //        }
        //    }

        //    processList(true, deviceIds);
        //}

        public static async void GetDevices()
        {
 
            HttpClient client = new HttpClient();

            //client.BaseAddress = new Uri(WebAPIEndpoint);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + apiKey);

            var request = new HttpRequestMessage(HttpMethod.Post, WebAPIEndpoint);
            request.Headers.Add("Authorization", WebAPIHeaderAuthorization);

            request.Content = new StringContent("{\"query\":\"select * from devices\"}", Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(request);

            //HttpResponseMessage response = await client.PostAsync(requestUrl, request.Content); //// (requestUrl, model);
            //string responseBody = await response.Content.ReadAsStringAsync();

            var responseString = await response.Content.ReadAsStringAsync();

        }
    }
}

