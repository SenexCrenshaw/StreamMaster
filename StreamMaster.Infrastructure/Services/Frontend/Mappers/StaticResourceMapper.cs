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

            bool test = !resourceUrl.Contains("/content/images/icons/manifest") &&
              !resourceUrl.Contains("/content/images/icons/browserconfig")
              && (resourceUrl.Contains("/static/")
              || resourceUrl.Contains("/content/")
              || resourceUrl.Contains("/assets/")) &&
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

            return test;
        }

        public override Task<string> MapAsync(string resourceUrl)
        {
            string normalizedUrl = NormalizeUrl(resourceUrl);
            string path = normalizedUrl.Replace('/', Path.DirectorySeparatorChar);
            string baseDir = Path.Combine(BuildInfo.StartUpPath, settings.UiFolder);
            return Task.FromResult(baseDir + path);
        }

        public static string NormalizeUrl(string resourceUrl)
        {
            // List of prefixes to look for
            string[] validPrefixes = ["/static/", "/content/", "/assets/"];

            foreach (string? prefix in validPrefixes)
            {
                int index = resourceUrl.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    // Return the substring starting from the valid prefix
                    return resourceUrl[index..];
                }
            }

            // If no valid prefix is found, return the original URL
            return resourceUrl;
        }
    }
}