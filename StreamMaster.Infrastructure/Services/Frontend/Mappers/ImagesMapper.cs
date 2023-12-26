using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.EnvironmentInfo;
using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class ImagesMapper(IAppFolderInfo appFolderInfo, ILogger<ImagesMapper> logger, ISettingsService settingsService) : StaticResourceMapperBase(logger)
    {
        public override bool CanHandle(string resourceUrl)
        {
            resourceUrl = resourceUrl.ToLowerInvariant();

            return resourceUrl.StartsWith("/images/") &&
                   (resourceUrl.EndsWith(".jpg") || resourceUrl.EndsWith(".png") || resourceUrl.EndsWith(".gif"));
        }

        public override async Task<string> Map(string resourceUrl)
        {
            Setting setting = await settingsService.GetSettingsAsync();
            string path = resourceUrl.Replace("/images/", "");

            string ret = Path.Combine(appFolderInfo.StartUpFolder, setting.UiFolder, "images", path);
            return ret;
        }
    }
}