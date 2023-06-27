using Microsoft.Extensions.Logging;

using StreamMasterDomain.Configuration;
using StreamMasterDomain.EnvironmentInfo;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class ManifestJsonMapper : StaticResourceMapperBase
    {
        private readonly IAppFolderInfo _appFolderInfo;
        private readonly IConfigFileProvider _configFileProvider;

        public ManifestJsonMapper(IAppFolderInfo appFolderInfo, IConfigFileProvider configFileProvider, ILogger<ManifestJsonMapper> logger)
            : base(logger)
        {
            _appFolderInfo = appFolderInfo;
            _configFileProvider = configFileProvider;
        }

        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.Equals("/manifest.json");
        }

        public override string Map(string resourceUrl)
        {
            return Path.Combine(_appFolderInfo.StartUpFolder, _configFileProvider.Setting.UiFolder, "manifest.json");
        }
    }
}
