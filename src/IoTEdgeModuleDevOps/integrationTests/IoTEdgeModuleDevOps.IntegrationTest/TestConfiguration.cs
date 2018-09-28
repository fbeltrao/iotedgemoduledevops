using System;
using Microsoft.Extensions.Configuration;

namespace IoTEdgeModuleDevOps.IntegrationTest
{
    public class TestConfiguration
    {
        public static IConfigurationRoot GetConfigurationRoot(string basePath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static T GetConfiguration<T>(string basePath, string section)
        {
            T result = default(T);

            GetConfigurationRoot(basePath).GetSection(section).Bind(result);

            return result;
        }
        
        public static TestConfiguration GetConfiguration()
        {
            var result = new TestConfiguration();

            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.local.json", optional: true)
                .AddEnvironmentVariables()
                .Build()
                .GetSection("testConfiguration")
                .Bind(result);

            return result;
        }



        public string IoTHubEventHubConnectionString { get; set; }
        public string IoTHubConnectionString { get; set; }  
        public string DeviceClientConnectionString { get; set; }
        public string CertificatePath { get; set; } = "/home/pi/iotedgecerts/certs/azure-iot-test-only.root.ca.cert.pem";

        public int EnsureHasEventDelayBetweenReadsInSeconds { get; set; } = 2;
        public int EnsureHasEventMaximumTries { get; set; } = 5;
        public string IoTHubEventHubConsumerGroup { get; set; } = "$Default";
    }

}