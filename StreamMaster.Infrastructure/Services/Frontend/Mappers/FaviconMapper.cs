using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class FaviconMapper(ILogger<FaviconMapper> logger, IOptionsMonitor<Setting> intsettings) : StaticResourceMapperBase(logger)
    {
        private readonly Setting settings = intsettings.CurrentValue;

        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.Equals("/favicon.ico");
        }

        public override async Task<string> Map(string resourceUrl)
        {

            return Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, "favicon.ico");
        }
    }
}