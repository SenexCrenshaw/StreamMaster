using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.Configuration;
using StreamMasterDomain.EnvironmentInfo;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class LoginHtmlMapper : HtmlMapperBase
    {
        protected Setting _setting = FileUtil.GetSetting();

        public LoginHtmlMapper(IAppFolderInfo appFolderInfo,

                               ILogger<LoginHtmlMapper> logger)
            : base(logger)
        {
            HtmlPath = Path.Combine(appFolderInfo.StartUpFolder, _setting.UiFolder, "login.html");
            UrlBase = _setting.UrlBase;
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