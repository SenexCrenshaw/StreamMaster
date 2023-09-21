using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.EnvironmentInfo;
using StreamMasterDomain.Services;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class LoginHtmlMapper(IAppFolderInfo appFolderInfo, ISettingsService settingsService, ILogger<LoginHtmlMapper> logger) : HtmlMapperBase(logger)
    {
        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.StartsWith("/login");
        }

        public override async Task<string> Map(string resourceUrl)
        {
            Setting setting = await settingsService.GetSettingsAsync();
            HtmlPath = Path.Combine(appFolderInfo.StartUpFolder, setting.UiFolder, "login.html");
            return HtmlPath;
        }
    }
}