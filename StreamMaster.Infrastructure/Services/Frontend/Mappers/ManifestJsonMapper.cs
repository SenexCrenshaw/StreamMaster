using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class ManifestJsonMapper(ILogger<ManifestJsonMapper> logger, IOptionsMonitor<Setting> intsettings) : StaticResourceMapperBase(logger)
    {
        private readonly Setting settings = intsettings.CurrentValue;

        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.Equals("/manifest.json");
        }

        public override async Task<string> Map(string resourceUrl)
        {
            return Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, "manifest.json");
        }
    }
}