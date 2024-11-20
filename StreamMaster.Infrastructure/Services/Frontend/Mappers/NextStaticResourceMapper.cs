using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class NextStaticResourceMapper(IOptionsMonitor<Setting> intSettings, ILogger<NextStaticResourceMapper> logger) : StaticResourceMapperBase(logger)
    {
        private readonly Setting settings = intSettings.CurrentValue;

        public override bool CanHandle(string resourceUrl)
        {
            resourceUrl = resourceUrl.ToLowerInvariant();
            if (resourceUrl.EndsWith("txt"))
            {
                _ = resourceUrl.EndsWith(".txt") || (resourceUrl.StartsWith("/_next/static/") &&
                (
                   (resourceUrl.EndsWith(".js") && !resourceUrl.EndsWith("initialize.js ")) ||
                   resourceUrl.EndsWith(".map") ||

                   resourceUrl.EndsWith(".woff2") ||
                   resourceUrl.EndsWith(".woff") ||
                   resourceUrl.EndsWith(".ttf") ||
                   resourceUrl.EndsWith(".css") ||
                       resourceUrl.EndsWith(".eot") ||
                   (resourceUrl.EndsWith(".ico") && !resourceUrl.Equals("/favicon.ico")) ||
                   resourceUrl.EndsWith(".swf") ||
                   resourceUrl.EndsWith("oauth.html"
                   )
                   ));
            }
            return resourceUrl.EndsWith(".txt") || (resourceUrl.StartsWith("/_next/static/") &&
                (
                   (resourceUrl.EndsWith(".js") && !resourceUrl.EndsWith("initialize.js ")) ||
                   resourceUrl.EndsWith(".map") ||

                   resourceUrl.EndsWith(".woff2") ||
                   resourceUrl.EndsWith(".woff") ||
                   resourceUrl.EndsWith(".ttf") ||
                   resourceUrl.EndsWith(".css") ||
                       resourceUrl.EndsWith(".eot") ||
                   (resourceUrl.EndsWith(".ico") && !resourceUrl.Equals("/favicon.ico")) ||
                   resourceUrl.EndsWith(".swf") ||
                   resourceUrl.EndsWith("oauth.html"
                   )
                   ));
        }

        public override Task<string> MapAsync(string resourceUrl)
        {
            string path = resourceUrl.Replace('/', Path.DirectorySeparatorChar);

            path = path.Trim(Path.DirectorySeparatorChar);

            return Task.FromResult(Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, path));
        }
    }
}