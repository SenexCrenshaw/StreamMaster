using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.EnvironmentInfo;
using StreamMasterDomain.Services;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class StaticResourceMapper(IAppFolderInfo appFolderInfo, ISettingsService settingsService, ILogger<NextStaticResourceMapper> logger) : StaticResourceMapperBase(logger)
    {
        public override bool CanHandle(string resourceUrl)
        {
            resourceUrl = resourceUrl.ToLowerInvariant();

            if (resourceUrl.StartsWith("/content/images/icons/manifest") ||
                resourceUrl.StartsWith("/content/images/icons/browserconfig"))
            {
                return false;
            }

            return (resourceUrl.StartsWith("/static/") || resourceUrl.StartsWith("/content/") || resourceUrl.StartsWith("/assets/")) &&
                (
                   (resourceUrl.EndsWith(".js") && !resourceUrl.EndsWith("initialize.js")) ||
                   resourceUrl.EndsWith(".map") ||
                   resourceUrl.EndsWith(".woff2") ||
                   resourceUrl.EndsWith(".woff") ||
                   resourceUrl.EndsWith(".ttf") ||
                   resourceUrl.EndsWith(".css") ||
                       resourceUrl.EndsWith(".eot") ||
                   (resourceUrl.EndsWith(".ico") && !resourceUrl.Equals("/favicon.ico")) ||
                   resourceUrl.EndsWith(".swf") ||
                   resourceUrl.EndsWith("oauth.html"
                   )
                   );
        }

        public override async Task<string> Map(string resourceUrl)
        {
            Setting setting = await settingsService.GetSettingsAsync();
            string path = resourceUrl.Replace('/', Path.DirectorySeparatorChar);
            path = path.Trim(Path.DirectorySeparatorChar);

            return Path.Combine(appFolderInfo.StartUpFolder, setting.UiFolder, path);
        }
    }
}