

using gbac_baseball.web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Configuration;

namespace gbac_baseball.web.Events
{

    public class BaseballEventProcessorFactory : IEventProcessorFactory
    {
        private readonly IHubContext<BaseballHub, IBaseball> _hub;
        private readonly IConfiguration _config;
        public BaseballEventProcessorFactory(IHubContext<BaseballHub, IBaseball> hub, IConfiguration config)
        {
            _hub = hub;
            _config = config;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context) =>
            new BaseballEventProcessor(_hub, _config);
    }
}