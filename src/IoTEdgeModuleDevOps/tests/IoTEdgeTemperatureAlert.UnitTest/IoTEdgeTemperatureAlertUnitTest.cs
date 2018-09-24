using Xunit;
using IoTEdgeTemperatureAlert;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;

namespace IoTEdgeTemperatureAlert.UnitTest
{
    public class IoTEdgeTemperatureAlertUnitTest
    {
        [Fact]
        public void FilterLessThanThresholdTest()
        {
            var source = CreateMessage(25 - 1);
            var result = Program.Filter(source);
            Assert.True(result == null);
        }

        [Fact]
        public void FilterMoreThanThresholdAlertPropertyTest()
        {
            var source = CreateMessage(25 + 1);
            var result = Program.Filter(source);
            Assert.True(result.Properties["MessageType"] == "Alert");
        }

        [Fact]
        public void FilterMoreThanThresholdCopyPropertyTest()
        {
            var source = CreateMessage(25 + 1);
            source.Properties.Add("customTestKey", "customTestValue");
            var result = Program.Filter(source);
            Assert.True(result.Properties["customTestKey"] == "customTestValue");
        }

        private Message CreateMessage(int temperature)
        {
            var messageBody = CreateMessageBody(temperature);
            var messageString = JsonConvert.SerializeObject(messageBody);
            var messageBytes = Encoding.UTF8.GetBytes(messageString);
            return new Message(messageBytes);
        }

        private MessageBody CreateMessageBody(int temperature)
        {
            var messageBody = new MessageBody
            {
                machine = new Machine
                {
                    temperature = temperature,
                    pressure = 0
                },
                ambient = new Ambient
                {
                    temperature = 0,
                    humidity = 0
                },
                timeCreated = string.Format("{0:O}", DateTime.Now)
            };

            return messageBody;
        }
    }
}
