using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using StackExchange.Redis;

namespace ThrottlingApi.Middleware
{
    public class RequestThrottlingMiddleware
    {

        public IConnectionMultiplexer connection;

        private RequestDelegate next;

        private int requestsPerMinuteThreshold;

        public RequestThrottlingMiddleware(RequestDelegate next, IConnectionMultiplexer connection, int requestsPerMinuteThreshold)
        {
            this.next = next;
            this.connection = connection;
            this.requestsPerMinuteThreshold = requestsPerMinuteThreshold;
        }

        public async Task Invoke(HttpContext context)
        {
            var cache = connection.GetDatabase();

            // Get this from the context in whatever way the user supplies it
            var consumerKey = Guid.NewGuid().ToString();

            var consumerCacheKey = $"consumer.throttle#{consumerKey}";

            var cacheResult = cache.HashIncrement(consumerCacheKey, 1);

            if (cacheResult == 1)
            {
                cache.KeyExpire($"consumer.throttle#{consumerKey}", TimeSpan.FromSeconds(15), CommandFlags.FireAndForget);
            }
            else if (cacheResult > requestsPerMinuteThreshold)
            {
                context.Response.StatusCode = 429;

                using (var writer = new StreamWriter(context.Response.Body))
                {
                    await writer.WriteAsync("You are making too many requests.");
                }

                return;
            }

            await next(context);
        }
    }
}