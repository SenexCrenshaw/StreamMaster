using Microsoft.Extensions.Logging;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
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

        public override string Map(string resourceUrl)
        {
            //var path = resourceUrl.Replace("/images/", "");

            //var ret = Path.Combine(_appFolderInfo.StartUpFolder, _setting.UiFolder, "images", path);
            return resourceUrl;
        }
    }
}