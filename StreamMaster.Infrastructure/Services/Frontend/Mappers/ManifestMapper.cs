using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class ManifestMapper(IOptionsMonitor<Setting> intSettings, ILogger<ManifestMapper> logger) : StaticResourceMapperBase(logger)
    {
        private readonly Setting settings = intSettings.CurrentValue;

        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.StartsWith("/Content/Images/Icons/manifest");
        }

        public override Task<string> MapAsync(string resourceUrl)
        {

            string path = resourceUrl.Replace('/', Path.DirectorySeparatorChar);
            path = path.Trim(Path.DirectorySeparatorChar);

            return Task.FromResult(Path.ChangeExtension(Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, path), "json"));
        }
    }
}