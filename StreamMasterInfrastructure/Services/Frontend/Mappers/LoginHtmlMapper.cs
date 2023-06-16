using Microsoft.Extensions.Logging;

using StreamMasterDomain.Configuration;
using StreamMasterDomain.EnvironmentInfo;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class LoginHtmlMapper : HtmlMapperBase
    {
        public LoginHtmlMapper(IAppFolderInfo appFolderInfo,
                               IConfigFileProvider configFileProvider,
                               ILogger logger)
            : base(logger)
        {
            HtmlPath = Path.Combine(appFolderInfo.StartUpFolder, configFileProvider.Setting.UiFolder, "login.html");
            UrlBase = "";// configFileProvider.UrlBase;
        }

        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.StartsWith("/login");
        }

        public override string Map(string resourceUrl)
        {
            return HtmlPath;
        }
    }
}
