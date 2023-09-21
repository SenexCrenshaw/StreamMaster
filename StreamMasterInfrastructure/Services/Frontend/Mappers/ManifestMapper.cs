using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.EnvironmentInfo;
using StreamMasterDomain.Services;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
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