using Microsoft.Extensions.Logging;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public class SwaggerMapper : StaticResourceMapperBase
    {

        public SwaggerMapper(ILogger<SwaggerMapper> logger) : base(logger)
        {

        }

        public override bool CanHandle(string resourceUrl)
        {
            resourceUrl = resourceUrl.ToLowerInvariant();

            return resourceUrl.StartsWith("/swagger/");
        }

        public override Task<string> MapAsync(string resourceUrl)
        {
            //var path = resourceUrl.Replace("/images/", "");

            //var ret = Path.Combine(_appFolderInfo.StartUpPath, _setting.UiFolder, "images", path);
            return Task.FromResult(resourceUrl);
        }
    }
}