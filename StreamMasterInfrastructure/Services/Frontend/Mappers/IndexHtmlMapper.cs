using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.EnvironmentInfo;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class IndexHtmlMapper : HtmlMapperBase
    {
        protected Setting _setting = FileUtil.GetSetting();

        public IndexHtmlMapper(IAppFolderInfo appFolderInfo,
                               ILogger<IndexHtmlMapper> logger)
            : base(logger)
        {
            HtmlPath = Path.Combine(appFolderInfo.StartUpFolder, _setting.UiFolder, "index.html");
            UrlBase = _setting.UrlBase;
        }

        public override bool CanHandle(string resourceUrl)
        {
            resourceUrl = resourceUrl.ToLowerInvariant();

            return !resourceUrl.StartsWith("/content") &&
                   !resourceUrl.StartsWith("/mediacover") &&
                   !resourceUrl.Contains('.') &&
                   !resourceUrl.StartsWith("/login");
        }

        public override string Map(string resourceUrl)
        {
            return HtmlPath;
        }
    }
}