using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.EnvironmentInfo;
using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class ManifestJsonMapper(IAppFolderInfo appFolderInfo, ILogger<ManifestJsonMapper> logger, ISettingsService settingsService) : StaticResourceMapperBase(logger)
    {
        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.Equals("/manifest.json");
        }

        public override async Task<string> Map(string resourceUrl)
        {
            Setting setting = await settingsService.GetSettingsAsync();
            return Path.Combine(appFolderInfo.StartUpFolder, setting.UiFolder, "manifest.json");
        }
    }
}