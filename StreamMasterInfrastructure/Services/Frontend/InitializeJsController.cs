using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMasterDomain.Common;
using StreamMasterDomain.Configuration;
using StreamMasterDomain.Dto;

using System.Text;

namespace StreamMasterInfrastructure.Services.Frontend
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = "UI")]
    [ApiController]
    public class InitializeJsController : Controller
    {
        private static string _apiKey;
        private static string _urlBase;

        public InitializeJsController(IConfigFileProvider configFileProvider)
        {
            var setting = FileUtil.GetSetting();
            _apiKey = setting.ApiKey;
            _urlBase = setting.UrlBase;
        }

        [HttpGet("/initialize.js")]
        public IActionResult Index()
        {
            return Content(GetContent(), "application/javascript");
        }

        private string GetContent()
        {
            var settingDto = new SettingDto();

            var builder = new StringBuilder();
            builder.AppendLine("window.StreamMaster = {");
            builder.AppendLine($"  apiKey: '{_apiKey}',");
            builder.AppendLine($"  apiRoot: '{_urlBase}/api/',");            
            builder.AppendLine($"  baseHostURL: '{_urlBase}',");            
            builder.AppendLine($"  isDebug: {BuildInfo.IsDebug.ToString()},");
            builder.AppendLine($"  urlBase: '{_urlBase}',");
            builder.AppendLine($"  version: '{BuildInfo.Version.ToString()}',");
            builder.AppendLine("};");

            return builder.ToString(); ;
        }
    }
}