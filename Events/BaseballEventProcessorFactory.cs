

using gbac_baseball.web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.EventHubs.Processor;

namespace gbac_baseball.web.Events
{

    public class BaseballEventProcessorFactory : IEventProcessorFactory
    {
        private readonly IHubContext<BaseballHub, IBaseball> _hub;
        public BaseballEventProcessorFactory(IHubContext<BaseballHub, IBaseball> hub) =>
            _hub = hub;

        public IEventProcessor CreateEventProcessor(PartitionContext context) =>
            new BaseballEventProcessor(_hub);
    }
}