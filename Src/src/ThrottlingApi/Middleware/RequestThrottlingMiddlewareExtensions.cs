using Microsoft.AspNet.Builder;

namespace ThrottlingApi.Middleware
{
    public static class RequestThrottlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestThrottling(
            this IApplicationBuilder app,
            int requestsPerMinuteThreshold = 100)
        {
            return app.UseMiddleware<RequestThrottlingMiddleware>(requestsPerMinuteThreshold);
        }
    }
}