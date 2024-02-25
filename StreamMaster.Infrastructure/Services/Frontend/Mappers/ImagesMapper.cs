using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class ImagesMapper(ILogger<ImagesMapper> logger, ISettingsService settingsService) : StaticResourceMapperBase(logger)
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

            string ret = Path.Combine(BuildInfo.StartUpPath, setting.UiFolder, "images", path);
            return ret;
        }
    }
}