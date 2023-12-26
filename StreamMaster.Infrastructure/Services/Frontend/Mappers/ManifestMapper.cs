using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.EnvironmentInfo;
using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class ManifestMapper(IAppFolderInfo appFolderInfo, ISettingsService settingsService, ILogger<ManifestMapper> logger) : StaticResourceMapperBase(logger)
    {
        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.StartsWith("/Content/Images/Icons/manifest");
        }

        public override async Task<string> Map(string resourceUrl)
        {
            Setting setting = await settingsService.GetSettingsAsync();
            string path = resourceUrl.Replace('/', Path.DirectorySeparatorChar);
            path = path.Trim(Path.DirectorySeparatorChar);

            return Path.ChangeExtension(Path.Combine(appFolderInfo.StartUpFolder, setting.UiFolder, path), "json");
        }
    }
}