using System;
using Xunit;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;
using IoTEdgeTemperatureAlert;
using Newtonsoft.Json;


namespace IoTEdgeModuleDevOps.IntegrationTest
{

    public class IoTEdgeModuleDevOpsIntegrationTest
    {
        //static DeviceClient client = DeviceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("TEST_IOT_DEVICE_CONNECTIONSTRING"));
         
        public IoTEdgeModuleDevOpsIntegrationTest()
        {
        }

        [Fact]
        public void Run_IntegrationTest_Smoketest()
        {
            Console.WriteLine("Running test in " + Environment.MachineName);
        }

        // [Fact]
        // public async Task SendMessageIsReceived()
        // {
        //     var messageBody = new MessageBody
        //     {
        //         machine = new Machine
        //         {
        //             temperature = 30,
        //             pressure = 0
        //         },
        //         ambient = new Ambient
        //         {
        //             temperature = 0,
        //             humidity = 0
        //         },
        //         timeCreated = string.Format("{0:O}", DateTime.Now)
        //     };

        //     await client.SendEventAsync(new Message(System.Text.UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageBody)))
        //     {
        //         ContentType = "application/json",
        //         ContentEncoding = "utf-8"                
        //     });            
        // }
    }
}
