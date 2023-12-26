using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.EnvironmentInfo;
using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class FaviconMapper(IAppFolderInfo appFolderInfo, ILogger<FaviconMapper> logger, ISettingsService settingsService) : StaticResourceMapperBase(logger)
    {
        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.Equals("/favicon.ico");
        }

        public override async Task<string> Map(string resourceUrl)
        {
            Setting setting = await settingsService.GetSettingsAsync();
            return Path.Combine(appFolderInfo.StartUpFolder, setting.UiFolder, "favicon.ico");
        }
    }
}