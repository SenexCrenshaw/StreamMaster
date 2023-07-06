using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.EnvironmentInfo;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class FaviconMapper : StaticResourceMapperBase
    {
        private readonly IAppFolderInfo _appFolderInfo;
        protected Setting _setting = FileUtil.GetSetting();

        public FaviconMapper(IAppFolderInfo appFolderInfo, ILogger<FaviconMapper> logger)
            : base(logger)
        {
            _appFolderInfo = appFolderInfo;
        }

        public override bool CanHandle(string resourceUrl)
        {
            return resourceUrl.Equals("/favicon.ico");
        }

        public override string Map(string resourceUrl)
        {
            return Path.Combine(_appFolderInfo.StartUpFolder, _setting.UiFolder, "favicon.ico");
        }
    }
}