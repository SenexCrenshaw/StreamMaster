using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Icons.Commands;
using StreamMasterApplication.Services;

namespace StreamMasterAPI.Controllers;

public class MiscController : ApiControllerBase
{
    private readonly IBackgroundTaskQueue _taskQueue;

    public MiscController(IBackgroundTaskQueue taskQueue)
    {
        _taskQueue = taskQueue;
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CacheAllIcons()
    {
        await _taskQueue.CacheAllIcons().ConfigureAwait(false);

        return NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CacheIconsFromProgrammes()
    {
        _ = await Mediator.Send(new CacheIconsFromProgrammesRequest()).ConfigureAwait(false);
        //await _taskQueue.CacheIconsFromProgrammes().ConfigureAwait(false);
        return NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CacheIconsFromVideoStreams()
    {
        await _taskQueue.CacheIconsFromVideoStreams().ConfigureAwait(false);
        return NoContent();
    }

    [HttpPut]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ReadDirectoryLogosRequest()
    {
        await _taskQueue.ReadDirectoryLogosRequest().ConfigureAwait(false);

        return NoContent();
    }
}
