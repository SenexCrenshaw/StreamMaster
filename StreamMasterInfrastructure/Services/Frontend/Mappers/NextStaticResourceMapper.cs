using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.EnvironmentInfo;
using StreamMasterDomain.Services;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class NextStaticResourceMapper(IAppFolderInfo appFolderInfo, ISettingsService settingsService, ILogger<NextStaticResourceMapper> logger) : StaticResourceMapperBase(logger)
    {
        public override bool CanHandle(string resourceUrl)
        {
            resourceUrl = resourceUrl.ToLowerInvariant();
            if (resourceUrl.EndsWith("txt"))
            {
                bool aa = resourceUrl.EndsWith(".txt") || (resourceUrl.StartsWith("/_next/static/") &&
                (
                   (resourceUrl.EndsWith(".js") && !resourceUrl.EndsWith("initialize.js ")) ||
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
                   ));

            }
            return resourceUrl.EndsWith(".txt") || (resourceUrl.StartsWith("/_next/static/") &&
                (
                   (resourceUrl.EndsWith(".js") && !resourceUrl.EndsWith("initialize.js ")) ||
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
                   ));
        }

        public override async Task<string> Map(string resourceUrl)
        {
            Setting setting = await settingsService.GetSettingsAsync();

            string path = resourceUrl.Replace('/', Path.DirectorySeparatorChar);
            //if (path.StartsWith("\\_next\\"))
            //{
            //    path = path[7..];
            //}

            path = path.Trim(Path.DirectorySeparatorChar);

            return Path.Combine(appFolderInfo.StartUpFolder, setting.UiFolder, path);
        }
    }
}