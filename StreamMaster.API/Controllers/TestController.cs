using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.Services;
using StreamMaster.Application.Statistics.Commands;

namespace StreamMaster.API.Controllers;

public class TestController(IBackgroundTaskQueue backgroundTaskQueue, ILogoService logoService, IChannelGroupService channelGroupService) : ApiControllerBase
{
    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> ScanDirectoryForM3UFiles()
    {
        await Mediator.Send(new ScanDirectoryForM3UFilesRequest()).ConfigureAwait(false);
        return Ok();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> ScanForTvLogos()
    {
        await logoService.ScanForTvLogosAsync();
        return Ok();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> AddTestTask(int DelayInSeconds)
    {
        await backgroundTaskQueue.SetTestTask(DelayInSeconds);
        return Ok();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> SetSystemReady(SetIsSystemReadyRequest request)
    {
        await Mediator.Send(request).ConfigureAwait(false);
        return Ok();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> UpdateChannelGroupCountsRequest()
    {
        await channelGroupService.UpdateChannelGroupCountsRequestAsync().ConfigureAwait(false);
        return Ok();
    }
}