using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class ImagesMapper(ILogger<ImagesMapper> logger, IOptionsMonitor<Setting> intsettings) : StaticResourceMapperBase(logger)
    {
        private readonly Setting settings = intsettings.CurrentValue;

        public override bool CanHandle(string resourceUrl)
        {
            resourceUrl = resourceUrl.ToLowerInvariant();

            return resourceUrl.StartsWith("/images/") &&
                   (resourceUrl.EndsWith(".jpg") || resourceUrl.EndsWith(".png") || resourceUrl.EndsWith(".gif"));
        }

        public override async Task<string> Map(string resourceUrl)
        {
            string path = resourceUrl.Replace("/images/", "");

            string ret = Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, "images", path);
            return ret;
        }
    }
}