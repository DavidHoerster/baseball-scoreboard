using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

using Newtonsoft.Json;

using gbac_baseball.web.Model;
using Microsoft.AspNetCore.SignalR;
using gbac_baseball.web.Hubs;

namespace gbac_baseball.web.Events
{

    public class BaseballEventProcessor : IEventProcessor
    {
        private readonly IHubContext<BaseballHub, IBaseball> _hub;
        public BaseballEventProcessor(IHubContext<BaseballHub, IBaseball> hub)
        {
            _hub = hub;
        }
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {

            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.CompletedTask;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                var gameEvent = JsonConvert.DeserializeObject<GameEvent>(data);

                await _hub.Clients.Group(gameEvent.HomeTeam).SendEvent(gameEvent.HomeTeam, gameEvent);

                Console.WriteLine($"Message received. Game {gameEvent.GameId} with score of {gameEvent.HomeTeam} {gameEvent.HomeScore} to {gameEvent.VisitingTeam} {gameEvent.VisitorScore}");
            }

            await context.CheckpointAsync();
        }
    }
}