using Microsoft.Extensions.Logging;

using StreamMasterDomain.Configuration;
using StreamMasterDomain.EnvironmentInfo;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class IndexHtmlMapper : HtmlMapperBase
    {
        private readonly IConfigFileProvider _configFileProvider;

        public IndexHtmlMapper(IAppFolderInfo appFolderInfo,
                               IConfigFileProvider configFileProvider,
                               ILogger<IndexHtmlMapper> logger)
            : base(logger)
        {
            _configFileProvider = configFileProvider;

            HtmlPath = Path.Combine(appFolderInfo.StartUpFolder, _configFileProvider.Setting.UiFolder, "index.html");
            UrlBase = "";
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
