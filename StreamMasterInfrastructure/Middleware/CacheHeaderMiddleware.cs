using Microsoft.AspNetCore.Http;

using StreamMasterInfrastructure.Extensions;

namespace StreamMasterInfrastructure.Middleware
{
    public class CacheHeaderMiddleware(RequestDelegate next, ICacheableSpecification cacheableSpecification)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method != "OPTIONS")
            {
                if (cacheableSpecification.IsCacheable(context.Request))
                {
                    context.Response.Headers.EnableCache();
                }
                else
                {
                    context.Response.Headers.DisableCache();
                }
            }

            await next(context);
        }
    }
}
