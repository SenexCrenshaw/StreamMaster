using Microsoft.AspNetCore.Http;

using StreamMaster.Infrastructure.Extensions;

namespace StreamMaster.Infrastructure.Middleware
{
    public class CacheHeaderMiddleware(RequestDelegate next, ICacheableSpecification cacheableSpecification)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method != "OPTIONS")
            {
                //if (cacheableSpecification.IsCacheable(context.Request))
                //{
                //    context.Response.Headers.EnableCache();
                //}
                //else
                //{
                context.Response.Headers.DisableCache();
                //}
            }

            await next(context);
        }
    }
}
