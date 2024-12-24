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

            return !resourceUrl.Contains("/content") &&
                   !resourceUrl.Contains("/mediacover") &&
                   !resourceUrl.Contains('.') &&
                   !resourceUrl.Contains("/login");
        }

        public override Task<string> MapAsync(string resourceUrl)
        {
            string HtmlPath = Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, "index.html");
            return Task.FromResult(HtmlPath);
        }
    }
}