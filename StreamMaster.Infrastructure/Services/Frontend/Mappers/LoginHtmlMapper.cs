using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class LoginHtmlMapper(IOptionsMonitor<Setting> intSettings, ILogger<LoginHtmlMapper> logger) : HtmlMapperBase(logger)
    {
        private readonly Setting settings = intSettings.CurrentValue;

        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.ContainsIgnoreCase("/login");
        }

        public override Task<string> MapAsync(string resourceUrl)
        {
            HtmlPath = Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, "login.html");
            return Task.FromResult(HtmlPath);
        }
    }
}