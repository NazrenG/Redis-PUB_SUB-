
using StackExchange.Redis;

namespace Redis_PUB_SUB_
{
    public class RedisSubcriberService : BackgroundService
    {
        private readonly ISubscriber subscriber;
        public RedisSubcriberService(IConfiguration configuration)
        {
            var muxer = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]);
            subscriber = muxer.GetSubscriber();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await subscriber.SubscribeAsync("channelList", async (channel, message) =>
            {
                Console.WriteLine($"Message received on {channel}: {message}");
            });
        }
    }
}
