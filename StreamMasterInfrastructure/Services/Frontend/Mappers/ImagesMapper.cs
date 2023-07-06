using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.EnvironmentInfo;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public class ImagesMapper : StaticResourceMapperBase
    {
        private readonly IAppFolderInfo _appFolderInfo;
        protected Setting _setting = FileUtil.GetSetting();

        public ImagesMapper(IAppFolderInfo appFolderInfo, ILogger<ImagesMapper> logger)
            : base(logger)
        {
            _appFolderInfo = appFolderInfo;
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

            var ret = Path.Combine(_appFolderInfo.StartUpFolder, _setting.UiFolder, "images", path);
            return ret;
        }
    }
}