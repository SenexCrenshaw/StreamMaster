using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class IndexHtmlMapper(IOptionsMonitor<Setting> intSettings,
                           ILogger<IndexHtmlMapper> logger) : HtmlMapperBase(logger)
    {
        private readonly Setting settings = intSettings.CurrentValue;

        public override bool CanHandle(string resourceUrl)
        {
            resourceUrl = resourceUrl.ToLowerInvariant();

            return !resourceUrl.StartsWith("/content") &&
                   !resourceUrl.StartsWith("/mediacover") &&
                   !resourceUrl.Contains('.') &&
                   !resourceUrl.StartsWith("/login");
        }

        public override async Task<string> Map(string resourceUrl)
        {

            string HtmlPath = Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, "index.html");
            return HtmlPath;
        }
    }
}