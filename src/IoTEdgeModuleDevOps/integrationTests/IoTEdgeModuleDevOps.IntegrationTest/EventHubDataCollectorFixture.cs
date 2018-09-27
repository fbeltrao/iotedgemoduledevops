using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace IoTEdgeModuleDevOps.IntegrationTest
{
    public class EventHubDataCollectorFixture : IDisposable
    {
        public const string MESSAGE_IDENTIFIER_PROPERTY_NAME = "messageIdentifier";


        private readonly TestConfiguration testConfiguration;

        public EventHubDataCollector Events { get; private set; }
        public DeviceClient DeviceClient { get; private set; }

        public EventHubDataCollectorFixture()
        {
            this.testConfiguration = TestConfiguration.GetConfiguration();

            this.Events = new EventHubDataCollector(testConfiguration.IoTHubEventHubConnectionString);
            var startTask = this.Events.Start();


            if (!string.IsNullOrEmpty(this.testConfiguration.CertificatePath) && File.Exists(this.testConfiguration.CertificatePath))
            {            
                var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadWrite);
                store.Add(new X509Certificate2(X509Certificate2.CreateFromCertFile(this.testConfiguration.CertificatePath)));
                store.Close();
            }
            
            this.DeviceClient = DeviceClient.CreateFromConnectionString(testConfiguration.DeviceClientConnectionString, Microsoft.Azure.Devices.Client.TransportType.Mqtt);

            startTask.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        internal string GetMessageIdentifier(EventData eventData) 
        {
            eventData.Properties.TryGetValue("messageIdentifier", out var actualMessageIdentifier);
            return actualMessageIdentifier?.ToString();
        }

        public async Task SendDeviceMessage(string messageId, object payloadData)
        {
            var payloadText = payloadData is string ? (string)payloadData : JsonConvert.SerializeObject(payloadData);
            Message message = new Message(System.Text.UTF8Encoding.UTF8.GetBytes(payloadText))
            {
                MessageId = messageId,
            };

            message.Properties.Add(MESSAGE_IDENTIFIER_PROPERTY_NAME, messageId);
            await this.DeviceClient.SendEventAsync(message);
        }

        public async Task<bool> EnsureHasEvent(Func<EventData, string, string, bool> predicate)
        {
            for (int i = 0; i < 5; i++)
            {
                foreach (var item in this.Events.GetEvents())
                {
                    var bodyText = System.Text.UTF8Encoding.UTF8.GetString(item.Body);
                    item.SystemProperties.TryGetValue("iothub-connection-device-id", out var deviceId);
                    if (predicate(item, deviceId?.ToString(), bodyText))
                    {
                        return true;
                    }

                }

                await Task.Delay(1000);
            }

            return false;
        }

        public void Dispose()
        {
            this.Events?.Dispose();
            this.Events = null;

            this.DeviceClient?.Dispose();
            this.DeviceClient = null;

            GC.SuppressFinalize(this);
        }
    }
}
