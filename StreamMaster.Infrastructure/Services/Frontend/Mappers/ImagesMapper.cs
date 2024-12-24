using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class ImagesMapper(ILogger<ImagesMapper> logger, IOptionsMonitor<Setting> intSettings) : StaticResourceMapperBase(logger)
    {
        private readonly Setting settings = intSettings.CurrentValue;

        public override bool CanHandle(string resourceUrl)
        {
            resourceUrl = resourceUrl.ToLowerInvariant();

            return resourceUrl.Contains("/images/") &&
                   (resourceUrl.EndsWith(".jpg") || resourceUrl.EndsWith(".png") || resourceUrl.EndsWith(".gif"));
        }

        public override Task<string> MapAsync(string resourceUrl)
        {
            string path = resourceUrl.Replace("/images/", "");

            string ret = Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, "images", path);
            return Task.FromResult(ret);
        }
    }
}