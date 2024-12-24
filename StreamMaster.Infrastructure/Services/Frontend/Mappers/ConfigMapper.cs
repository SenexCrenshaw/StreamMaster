using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class ConfigMapper(ILogger<ConfigMapper> logger, IOptionsMonitor<Setting> intSettings) : StaticResourceMapperBase(logger)
    {
        private readonly Setting settings = intSettings.CurrentValue;

        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.EndsWithIgnoreCase("/config.json");
        }

        public override Task<string> MapAsync(string resourceUrl)
        {
            return Task.FromResult(Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, "config.json"));
        }
    }
}