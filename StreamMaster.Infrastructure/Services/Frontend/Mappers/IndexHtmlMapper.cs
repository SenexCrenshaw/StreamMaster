using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.EnvironmentInfo;
using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class IndexHtmlMapper(IAppFolderInfo appFolderInfo, ISettingsService settingsService,
                           ILogger<IndexHtmlMapper> logger) : HtmlMapperBase(logger)
    {
        public override bool CanHandle(string resourceUrl)
        {
            resourceUrl = resourceUrl.ToLowerInvariant();

            return !resourceUrl.StartsWith("/content") &&
                   !resourceUrl.StartsWith("/mediacover") &&
                   !resourceUrl.Contains('.') &&
                   !resourceUrl.StartsWith("/login");
        }

        public override async Task<string> Map(string resourceUrl)
        {
            Setting setting = await settingsService.GetSettingsAsync();
            string HtmlPath = Path.Combine(appFolderInfo.StartUpFolder, setting.UiFolder, "index.html");
            return HtmlPath;
        }
    }
}