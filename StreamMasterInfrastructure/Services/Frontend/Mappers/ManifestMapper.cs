using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.EnvironmentInfo;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class ManifestMapper : StaticResourceMapperBase
    {
        private readonly IAppFolderInfo _appFolderInfo;
        protected Setting _setting = FileUtil.GetSetting();

        public ManifestMapper(IAppFolderInfo appFolderInfo, ILogger<ManifestMapper> logger)
            : base(logger)
        {
            _appFolderInfo = appFolderInfo;
        }

        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.StartsWith("/Content/Images/Icons/manifest");
        }

        public override string Map(string resourceUrl)
        {
            var path = resourceUrl.Replace('/', Path.DirectorySeparatorChar);
            path = path.Trim(Path.DirectorySeparatorChar);

            return Path.ChangeExtension(Path.Combine(_appFolderInfo.StartUpFolder, _setting.UiFolder, path), "json");
        }
    }
}