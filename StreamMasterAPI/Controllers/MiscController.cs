using MediatR;

using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Icons.Commands;
using StreamMasterApplication.Services;

namespace StreamMasterAPI.Controllers;

public class MiscController : ApiControllerBase
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ISender _sender;

    public MiscController(IBackgroundTaskQueue taskQueue, ISender sender)
    {
        _taskQueue = taskQueue;
        _sender = sender;
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> BuildIconsCacheFromVideoStreams()
    {
        await _taskQueue.BuildIconsCacheFromVideoStreams().ConfigureAwait(false);
        return NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> ReadDirectoryLogosRequest()
    {
        await _taskQueue.ReadDirectoryLogosRequest().ConfigureAwait(false);

        return NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<ActionResult> BuildProgIconsCacheFromEPGsRequest()
    {
        await _sender.Send(new BuildProgIconsCacheFromEPGsRequest()).ConfigureAwait(false);

        return NoContent();
    }
}