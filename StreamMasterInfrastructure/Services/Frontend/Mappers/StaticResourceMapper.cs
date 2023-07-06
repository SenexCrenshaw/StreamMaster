using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.EnvironmentInfo;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class StaticResourceMapper : StaticResourceMapperBase
    {
        private readonly IAppFolderInfo _appFolderInfo;

        public StaticResourceMapper(IAppFolderInfo appFolderInfo, ILogger<StaticResourceMapper> logger)
            : base(logger)
        {
            _appFolderInfo = appFolderInfo;
        }

        public override bool CanHandle(string resourceUrl)
        {
            resourceUrl = resourceUrl.ToLowerInvariant();

            if (resourceUrl.StartsWith("/content/images/icons/manifest") ||
                resourceUrl.StartsWith("/content/images/icons/browserconfig"))
            {
                return false;
            }

            return (resourceUrl.StartsWith("/static/") || resourceUrl.StartsWith("/content/")) &&
                (
                   resourceUrl.EndsWith(".js") && !resourceUrl.EndsWith("initialize.js") ||
                   resourceUrl.EndsWith(".map") ||
                   resourceUrl.EndsWith(".woff2") ||
                   resourceUrl.EndsWith(".woff") ||
                   resourceUrl.EndsWith(".ttf") ||
                   resourceUrl.EndsWith(".css") ||
                       resourceUrl.EndsWith(".eot") ||
                   resourceUrl.EndsWith(".ico") && !resourceUrl.Equals("/favicon.ico") ||
                   resourceUrl.EndsWith(".swf") ||
                   resourceUrl.EndsWith("oauth.html"
                   )
                   );
        }

        public override string Map(string resourceUrl)
        {
            var path = resourceUrl.Replace('/', Path.DirectorySeparatorChar);
            path = path.Trim(Path.DirectorySeparatorChar);
            var setting = FileUtil.GetSetting();
            return Path.Combine(_appFolderInfo.StartUpFolder, setting.UiFolder, path);
        }
    }
}