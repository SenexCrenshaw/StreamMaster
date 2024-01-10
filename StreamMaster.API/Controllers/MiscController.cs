using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Services;

using StreamMasterAPI.Controllers;

using System.Text;

namespace StreamMaster.API.Controllers;

public class MiscController : ApiControllerBase
{
    private readonly IImageDownloadService imageDownloadService;

    public MiscController(IImageDownloadService imageDownloadService)
    {
        this.imageDownloadService = imageDownloadService;
    }

    [HttpGet]
    [Route("[action]")]
    public ActionResult<ImageDownloadServiceStatus> GetDownloadServiceStatus()
    {
        ImageDownloadServiceStatus status = imageDownloadService.GetStatus();
        return Ok(status);
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult GetTestM3U(int numberOfStreams)
    {
        List<string> lines = [];
        lines.Add("#EXTM3U");
        for (int i = 0; i < numberOfStreams; i++)
        {
            string id = Guid.NewGuid().ToString();
            lines.Add($"#EXTINF:-1 tvg-id=\"Channel_{i}\" tvg-name=\"Channel {i}\" tvg-chno=\"{i}\" tvg-logo=\"https://logo{i}.png\" group-title =\"TEST CHANNEL GROUP\", Channel {i}");
            lines.Add($"http://channelfake.test/live/{id}.ts");
        }

        string data = string.Join("\r\n", lines);

        return new FileContentResult(Encoding.UTF8.GetBytes(data), "application/x-mpegURL")
        {
            FileDownloadName = $"m3u-test-{numberOfStreams}.m3u"
        };
    }
}