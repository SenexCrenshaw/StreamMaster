using Microsoft.Extensions.Logging;

using StreamMasterDomain.Configuration;
using StreamMasterDomain.EnvironmentInfo;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class ImagesMapper : StaticResourceMapperBase
    {
        private readonly IAppFolderInfo _appFolderInfo;
        private readonly IConfigFileProvider _configFileProvider;

        public ImagesMapper(IAppFolderInfo appFolderInfo, IConfigFileProvider configFileProvider, ILogger<ImagesMapper> logger)
            : base(logger)
        {
            _appFolderInfo = appFolderInfo;
            _configFileProvider = configFileProvider;
        }

        public override bool CanHandle(string resourceUrl)
        {
            resourceUrl = resourceUrl.ToLowerInvariant();

            return resourceUrl.StartsWith("/images/") &&
                   (resourceUrl.EndsWith(".jpg") || resourceUrl.EndsWith(".png") || resourceUrl.EndsWith(".gif"));
        }

        public override string Map(string resourceUrl)
        {
            var path = resourceUrl.Replace("/images/", "");

            var ret = Path.Combine(_appFolderInfo.StartUpFolder, _configFileProvider.Setting.UiFolder, "images", path);
            return ret;
        }
    }
}
