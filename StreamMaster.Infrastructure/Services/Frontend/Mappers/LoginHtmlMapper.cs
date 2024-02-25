using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class LoginHtmlMapper(ISettingsService settingsService, ILogger<LoginHtmlMapper> logger) : HtmlMapperBase(logger)
    {
        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.StartsWith("/login");
        }

        public override async Task<string> Map(string resourceUrl)
        {
            Setting setting = await settingsService.GetSettingsAsync();
            HtmlPath = Path.Combine(BuildInfo.StartUpPath, setting.UiFolder, "login.html");
            return HtmlPath;
        }
    }
}