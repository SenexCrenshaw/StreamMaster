using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class StaticResourceMapper(IOptionsMonitor<Setting> intSettings, ILogger<NextStaticResourceMapper> logger) : StaticResourceMapperBase(logger)
    {
        private readonly Setting settings = intSettings.CurrentValue;

        public override bool CanHandle(string resourceUrl)
        {
            resourceUrl = resourceUrl.ToLowerInvariant();

            return !resourceUrl.StartsWith("/content/images/icons/manifest") &&
!resourceUrl.StartsWith("/content/images/icons/browserconfig")
&& (resourceUrl.StartsWith("/static/") || resourceUrl.StartsWith("/content/") || resourceUrl.StartsWith("/assets/")) &&
                (
                   (resourceUrl.EndsWith(".js") && !resourceUrl.EndsWith("initialize.js")) ||
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
                   );
        }

        public override Task<string> MapAsync(string resourceUrl)
        {
            string path = resourceUrl.Replace('/', Path.DirectorySeparatorChar);
            path = path.Trim(Path.DirectorySeparatorChar);

            return Task.FromResult(Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, path));
        }
    }
}