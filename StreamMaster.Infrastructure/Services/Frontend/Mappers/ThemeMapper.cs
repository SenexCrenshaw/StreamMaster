using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers;

public class ThemeMapper(ILogger<ThemeMapper> logger, IOptionsMonitor<Setting> intsettings) : StaticResourceMapperBase(logger)
{
    private readonly Setting settings = intsettings.CurrentValue;

    public override bool CanHandle(string resourceUrl)
    {
        resourceUrl = resourceUrl.ToLowerInvariant();

        return resourceUrl.StartsWith("/themes/") &&
               (resourceUrl.EndsWith(".css") || resourceUrl.EndsWith(".woff") || resourceUrl.EndsWith(".woff2"));
    }

    public override async Task<string> Map(string resourceUrl)
    {
        string path = resourceUrl.Replace("/themes/", "");

        string ret = Path.Combine(BuildInfo.StartUpPath, settings.UiFolder, "themes", path);
        return ret;
    }
}