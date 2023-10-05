namespace StreamMasterInfrastructure.Services.Frontend
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    //[Authorize(Policy = "UI")]
    //[ApiController]
    //public class InitializeJsController(ISettingsService settingsService) : Controller
    //{
    //    [AllowAnonymous]
    //    [HttpGet("/initialize.js")]
    //    public async Task<IActionResult> Index()
    //    {
    //        return Content(await GetContent(), "application/javascript");
    //    }

    //    private async Task<string> GetContent()
    //    {
    //        Setting setting = await settingsService.GetSettingsAsync();
    //        StringBuilder builder = new();
    //        builder.AppendLine("window.StreamMaster = {");
    //        builder.AppendLine($"  apiKey: '{setting.ApiKey}',");
    //        builder.AppendLine($"  apiRoot: '{setting.UrlBase}/api/',");
    //        builder.AppendLine($"  baseHostURL: '{setting.UrlBase}',");
    //        builder.AppendLine($"  isDebug: {BuildInfo.IsDebug.ToString().ToLower()},");
    //        builder.AppendLine($"  urlBase: '{setting.UrlBase}',");
    //        builder.AppendLine($"  version: '{BuildInfo.Version.ToString()}',");
    //        builder.AppendLine("};");

    //        return builder.ToString();
    //    }
    //}
}