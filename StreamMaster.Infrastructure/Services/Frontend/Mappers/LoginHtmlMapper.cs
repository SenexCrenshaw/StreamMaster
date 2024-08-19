using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class LoginHtmlMapper(IOptionsMonitor<Setting> intSettings, ILogger<LoginHtmlMapper> logger) : HtmlMapperBase(logger)
    {
        private readonly Setting settings = intSettings.CurrentValue;

        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.StartsWith("/login");
        }

        public override async Task<string> Map(string resourceUrl)
        {
            HtmlPath = Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, "login.html");
            return HtmlPath;
        }
    }
}