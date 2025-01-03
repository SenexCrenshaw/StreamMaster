using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers;

public class ThemeMapper(ILogger<ThemeMapper> logger, IOptionsMonitor<Setting> intSettings) : StaticResourceMapperBase(logger)
{
    private readonly Setting settings = intSettings.CurrentValue;

    public override bool CanHandle(string resourceUrl)
    {
        resourceUrl = resourceUrl.ToLowerInvariant();

        return resourceUrl.Contains("/themes/") &&
               (resourceUrl.EndsWith(".css") || resourceUrl.EndsWith(".woff") || resourceUrl.EndsWith(".woff2"));
    }

    public override Task<string> MapAsync(string resourceUrl)
    {
        string path = resourceUrl.ExtractPath("/themes/");

        string ret = Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, "themes", path);
        return Task.FromResult(ret);
    }
}