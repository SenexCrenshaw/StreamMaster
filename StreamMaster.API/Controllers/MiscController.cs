using Microsoft.AspNetCore.Mvc;


using StreamMaster.Streams.Handlers;

using System.Text;

namespace StreamMaster.API.Controllers;

public class MiscController(IImageDownloadService imageDownloadService, IStreamBroadcasterService channelDistributorService) : ApiControllerBase
{
    [HttpGet]
    [Route("[action]")]
    public ActionResult<ImageDownloadServiceStatus> GetDownloadServiceStatus()
    {
        ImageDownloadServiceStatus status = imageDownloadService.GetStatus();
        return Ok(status);
    }

    [HttpGet]
    [Route("[action]")]
    public ActionResult<List<StreamBroadcaster>> GetChannelDiGetChannelDistributors()
    {
        List<IStreamBroadcaster> channelDistributors = channelDistributorService.GetStreamBroadcasters();
        return Ok(channelDistributors);
    }

    [HttpGet]
    [Route("[action]")]
    public ActionResult<IDictionary<string, IStreamHandlerMetrics>> GetAggregatedMetrics()
    {
        IDictionary<string, IStreamHandlerMetrics> metrics = channelDistributorService.GetMetrics();
        return Ok(metrics);
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
            lines.Add($"#EXTINF:-1 tvg-id=\"Channel_{i}\" tvg-name=\"Channel {i}\" tvg-chno=\"{i}\" tvg-logo=\"https://logo{i}.png\" group-title=\"TEST CHANNEL GROUP\", Channel {i}");
            lines.Add($"http://channelfake.test/live/{id}.ts");
        }

        string data = string.Join("\r\n", lines);

        return new FileContentResult(Encoding.UTF8.GetBytes(data), "application/x-mpegURL")
        {
            FileDownloadName = $"m3u-test-{numberOfStreams}.m3u"
        };
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> Backup()
    {
        await FileUtil.Backup();
        return Ok();
    }
}