using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.EnvironmentInfo;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class ManifestJsonMapper : StaticResourceMapperBase
    {
        private readonly IAppFolderInfo _appFolderInfo;

        public ManifestJsonMapper(IAppFolderInfo appFolderInfo, ILogger<ManifestJsonMapper> logger)
            : base(logger)
        {
            _appFolderInfo = appFolderInfo;
        }

        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.Equals("/manifest.json");
        }

        public override string Map(string resourceUrl)
        {
            var setting = FileUtil.GetSetting();
            return Path.Combine(_appFolderInfo.StartUpFolder, setting.UiFolder, "manifest.json");
        }
    }
}