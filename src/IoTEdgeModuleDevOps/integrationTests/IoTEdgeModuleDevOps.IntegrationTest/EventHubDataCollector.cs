using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace IoTEdgeModuleDevOps.IntegrationTest
{
    public class EventHubDataCollector : IPartitionReceiveHandler
    {
        private readonly EventHubClient eventHubClient;

        public string ConsumerGroupName { get; set; } = "$Default";
        

        public EventHubDataCollector(string connectionString)
        {
            this.eventHubClient = EventHubClient.CreateFromConnectionString("");
            
        }

        public async Task Start()
        {
            var rti = await this.eventHubClient.GetRuntimeInformationAsync();
            foreach (var partitionId in rti.PartitionIds)
            {
                var receiver = this.eventHubClient.CreateReceiver(this.ConsumerGroupName, partitionId, EventPosition.FromEnqueuedTime(DateTime.UtcNow))
                receiver.SetReceiveHandler(this);
                                
            }
        }

        Task IPartitionReceiveHandler.ProcessEventsAsync(IEnumerable<EventData> events)
        {
            throw new NotImplementedException();
        }

        Task IPartitionReceiveHandler.ProcessErrorAsync(Exception error)
        {
            throw new NotImplementedException();
        }

        int IPartitionReceiveHandler.MaxBatchSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
