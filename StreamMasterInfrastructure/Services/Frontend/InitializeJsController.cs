using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMasterDomain.Configuration;
using StreamMasterDomain.Dto;

using System.Text;

namespace StreamMasterInfrastructure.Services.Frontend
{
    [Authorize(Policy = "UI")]
    [ApiController]
    public class InitializeJsController : Controller
    {
        private static string _apiKey;
        private static string _urlBase;
        private readonly IConfigFileProvider _configFileProvider;
        private string _generatedContent;

        public InitializeJsController(IConfigFileProvider configFileProvider)
        {
            _configFileProvider = configFileProvider;

            _apiKey = configFileProvider.Setting.ApiKey;
            _urlBase = "";// configFileProvider.Setting.UrlBase;
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
            builder.AppendLine($"  apiRoot: '{_urlBase}/api/',");
            builder.AppendLine($"  apiKey: '{_apiKey}',");
            builder.AppendLine($"  baseHostURL: '{_urlBase}',");
            builder.AppendLine($"  isDev: false,");
            builder.AppendLine($"  requiresAuth: {(!string.IsNullOrEmpty(_configFileProvider.Setting.AdminPassword) && !string.IsNullOrEmpty(_configFileProvider.Setting.AdminUserName)).ToString().ToLower()},");
            builder.AppendLine($"  urlBase: '{_urlBase}',");
            builder.AppendLine($"  version: '{settingDto.Version.ToString()}',");
            builder.AppendLine("};");

            _generatedContent = builder.ToString();

            return _generatedContent;
        }
    }
}
