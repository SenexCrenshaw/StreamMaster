using Microsoft.Extensions.Logging;

using StreamMasterDomain.Configuration;
using StreamMasterDomain.EnvironmentInfo;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class FaviconMapper : StaticResourceMapperBase
    {
        private readonly IAppFolderInfo _appFolderInfo;
        private readonly IConfigFileProvider _configFileProvider;

        public FaviconMapper(IAppFolderInfo appFolderInfo, IConfigFileProvider configFileProvider, ILogger<FaviconMapper> logger)
            : base(logger)
        {
            _appFolderInfo = appFolderInfo;
            _configFileProvider = configFileProvider;
        }

        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.Equals("/favicon.ico");
        }

        public override string Map(string resourceUrl)
        {
            return Path.Combine(_appFolderInfo.StartUpFolder, _configFileProvider.Setting.UiFolder, "favicon.ico");
        }
    }
}
