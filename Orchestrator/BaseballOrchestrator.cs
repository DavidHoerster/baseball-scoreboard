using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using gbac_baseball.web.Hubs;
using gbac_baseball.web.Events;

namespace gbac_baseball.web.Orchestrator
{

    public class BaseballOrchestrator : BackgroundService
    {

        private readonly EventProcessorHost _eventProcessor;
        private readonly IHubContext<BaseballHub, IBaseball> _hub;
        public BaseballOrchestrator(IConfiguration config, IHubContext<BaseballHub, IBaseball> hub)
        {
            _eventProcessor = new EventProcessorHost(
                                        config["event-hub-hub-name"],
                                        PartitionReceiver.DefaultConsumerGroupName,
                                        config["event-hub-connection-string"],
                                        config["event-storage-connection-string"],
                                        config["event-storage-account-container"]);

            _hub = hub;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _eventProcessor.RegisterEventProcessorFactoryAsync(new BaseballEventProcessorFactory(_hub));
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(500);
            }
            await _eventProcessor.UnregisterEventProcessorAsync();
        }
    }
}