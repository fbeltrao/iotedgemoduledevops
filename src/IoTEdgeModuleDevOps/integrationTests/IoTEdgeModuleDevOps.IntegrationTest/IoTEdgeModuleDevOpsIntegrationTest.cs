using System;
using Xunit;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;
using IoTEdgeTemperatureAlert;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace IoTEdgeModuleDevOps.IntegrationTest
{

    public class IoTEdgeModuleDevOpsIntegrationTest : IClassFixture<EventHubDataCollectorFixture>
    {
        private readonly EventHubDataCollectorFixture fixture;        

        public IoTEdgeModuleDevOpsIntegrationTest(EventHubDataCollectorFixture fixture)
        {
            this.fixture = fixture;
        }


        [Fact]
        public async Task Normal_Temperature_Does_Not_Go_Upstream()
        {
            var payloadData = new { machine = new { temperature = 20, pressure = 0 }, ambient = new { temperature = 21, humidity = 0 } };
            var payloadText = JsonConvert.SerializeObject(payloadData);
            var messageId = Guid.NewGuid().ToString();

            await this.fixture.SendDeviceMessage(messageId, payloadText);
            
            var eventFound = await this.fixture.EnsureHasEvent((m, deviceId, messageBody) =>
            {                
                return messageId.Equals(this.fixture.GetMessageIdentifier(m));
            });

            Assert.False(eventFound, "Message was found in upstream");
        }

        [Fact]
        public async Task High_Temperature_Goes_Not_Go_Upstream()
        {
            var payloadData = new { machine = new { temperature = 30, pressure = 0 }, ambient = new { temperature = 21, humidity = 0 } };
            var payloadText = JsonConvert.SerializeObject(payloadData);
            var messageId = Guid.NewGuid().ToString();

            await this.fixture.SendDeviceMessage(messageId, payloadText);

            var eventFound = await this.fixture.EnsureHasEvent((m, deviceId, messageBody) =>
            {
                return messageId.Equals(this.fixture.GetMessageIdentifier(m));
            });

            Assert.True(eventFound, "Message was not found in upstream");
        }        
    }
}
