using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class ManifestMapper(IOptionsMonitor<Setting> intsettings, ILogger<ManifestMapper> logger) : StaticResourceMapperBase(logger)
    {
        private readonly Setting settings = intsettings.CurrentValue;

        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.StartsWith("/Content/Images/Icons/manifest");
        }

        public override async Task<string> Map(string resourceUrl)
        {

            string path = resourceUrl.Replace('/', Path.DirectorySeparatorChar);
            path = path.Trim(Path.DirectorySeparatorChar);

            return Path.ChangeExtension(Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, path), "json");
        }
    }
}