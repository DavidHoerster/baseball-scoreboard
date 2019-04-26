using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

using Newtonsoft.Json;
using Dapper;

using gbac_baseball.web.Model;
using Microsoft.AspNetCore.SignalR;
using gbac_baseball.web.Hubs;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using MySql.Data.MySqlClient;
using System.Data;

namespace gbac_baseball.web.Events
{
    public class BaseballEventProcessor : IEventProcessor
    {
        private readonly IHubContext<BaseballHub, IBaseball> _hub;
        private readonly IConfiguration _config;
        private readonly Lazy<ConnectionMultiplexer> _redisConnection;
        private IDatabase _cache;
        public BaseballEventProcessor(IHubContext<BaseballHub, IBaseball> hub, IConfiguration config)
        {
            _hub = hub;
            _config = config;

            _redisConnection = new Lazy<ConnectionMultiplexer>(() =>
                ConnectionMultiplexer.Connect(_config["Azure:Redis:CacheConnection"])
            );
            _cache = _redisConnection.Value.GetDatabase();
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
            using (var conn = new MySqlConnection(_config["Azure:MySql:ConnectionString"]))
            {
                foreach (var eventData in messages)
                {
                    var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    var gameEvent = JsonConvert.DeserializeObject<GameEvent>(data);

                    var updatedGameEvent = await GetPlayerNames(gameEvent, conn);

                    await _hub.Clients.Group(updatedGameEvent.HomeTeam).SendEvent(updatedGameEvent.HomeTeam, updatedGameEvent);

                    Console.WriteLine($"Message received. Game {updatedGameEvent.GameId} with score of {updatedGameEvent.HomeTeam} {updatedGameEvent.HomeScore} to {updatedGameEvent.VisitingTeam} {updatedGameEvent.VisitorScore}");
                }
            }
            await context.CheckpointAsync();
        }


        private async Task<GameEvent> GetPlayerNames(GameEvent evt, IDbConnection conn)
        {
            var pitcherName = await _cache.StringGetAsync(evt.Pitcher);
            if (pitcherName.IsNullOrEmpty)
            {
                //query the db
                var name = await conn.QuerySingleOrDefaultAsync("SELECT nameFirst, nameLast FROM people WHERE retroID=@playerId", new { playerId = evt.Pitcher });
                if (name != null)
                {
                    pitcherName = $"{name.nameFirst} {name.nameLast}";
                    await _cache.StringSetAsync(evt.Pitcher, pitcherName);
                }
            }

            var batterName = await _cache.StringGetAsync(evt.Batter);
            if (batterName.IsNullOrEmpty)
            {
                //query the db
                var name = await conn.QuerySingleOrDefaultAsync("SELECT nameFirst, nameLast FROM people WHERE retroID=@playerId", new { playerId = evt.Batter });
                if (name != null)
                {
                    batterName = $"{name.nameFirst} {name.nameLast}";
                    await _cache.StringSetAsync(evt.Batter, batterName);
                }
            }

            evt.Batter = batterName;
            evt.Pitcher = pitcherName;

            return evt;
        }
    }
}