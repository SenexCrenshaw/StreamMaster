using Microsoft.AspNetCore.Mvc;

using StreamMaster.Streams.Handlers;

using System.Text;

namespace StreamMaster.API.Controllers;

public class MiscController(IImageDownloadService imageDownloadService, IVideoInfoService videoInfoService, IChannelBroadcasterService channelDistributorService) : ApiControllerBase
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
    public ActionResult<List<ChannelBroadcaster>> GetChannelDiGetChannelDistributors()
    {
        List<IChannelBroadcaster> channelDistributors = channelDistributorService.GetChannelBroadcasters();
        return Ok(channelDistributors);
    }



    [HttpGet]
    [Route("[action]")]
    public ActionResult<List<VideoInfoDto>> GetVideoInfos()
    {
        System.Collections.Concurrent.ConcurrentDictionary<string, VideoInfo> infos = videoInfoService.VideoInfos;
        List<VideoInfoDto> ret = [];
        foreach (KeyValuePair<string, VideoInfo> info in infos)
        {
            ret.Add(new VideoInfoDto(info));
        }
        return Ok(ret);
    }




    [HttpGet]
    [Route("[action]")]
    public ActionResult<IDictionary<string, IStreamHandlerMetrics>> GetAggregatedMetrics()
    {
        IDictionary<string, IStreamHandlerMetrics> metrics = channelDistributorService.GetAggregatedMetrics();
        return Ok(metrics);
    }

    //[HttpGet("health")]
    //public IActionResult GetStreamManagerHealth()
    //{
    //    bool isHealthy = channelDistributorService.IsHealthy();
    //    return isHealthy ? Ok("Healthy") : StatusCode(500, "Unhealthy");
    //}

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