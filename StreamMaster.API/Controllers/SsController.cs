using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.StreamGroups.Queries;

using System.Text;

namespace StreamMaster.API.Controllers;

[V1ApiController("s")]
public class SsController(ISender Sender, IStreamGroupService streamGroupService, IHttpContextAccessor httpContextAccessor) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    [Route("{streamGroupProfileId}")]
    [Route("{streamGroupProfileId}/capability")]
    [Route("{streamGroupProfileId}/device.xml")]
    public async Task<IActionResult> GetStreamGroupCapability(int streamGroupProfileId)
    {
        if (httpContextAccessor?.HttpContext?.Request == null)
        {
            return NotFound();
        }

        string xml = await streamGroupService.GetStreamGroupCapability(streamGroupProfileId, httpContextAccessor.HttpContext.Request).ConfigureAwait(false);
        return new ContentResult
        {
            Content = xml,
            ContentType = "application/xml",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Authorize(Policy = "SGLinks")]
    [Route("{streamGroupProfileId}/discover.json")]
    public async Task<IActionResult> GetStreamGroupDiscover(int streamGroupProfileId)
    {

        if (HttpContext.Request == null)
        {
            return NotFound();
        }

        string json = await streamGroupService.GetStreamGroupDiscover(streamGroupProfileId, HttpContext.Request).ConfigureAwait(false);

        return new ContentResult
        {
            Content = json,
            ContentType = "text/json",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Authorize(Policy = "SGLinks")]
    [Route("{streamGroupProfileId}/lineup.json")]
    public async Task<IActionResult> GetStreamGroupLineup(int streamGroupProfileId)
    {
        if (HttpContext.Request == null)
        {
            return NotFound();
        }
        string json = await streamGroupService.GetStreamGroupLineup(streamGroupProfileId, HttpContext.Request, true).ConfigureAwait(false);
        return new ContentResult
        {
            Content = json,
            ContentType = "application/json",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Authorize(Policy = "SGLinks")]
    [Route("{streamGroupProfileId}/lineup_status.json")]
    public Task<IActionResult> GetStreamGroupLineupStatus(int streamGroupProfileId)
    {
        string json = streamGroupService.GetStreamGroupLineupStatus();
        return Task.FromResult<IActionResult>(new ContentResult
        {
            Content = json,
            ContentType = "text/json",
            StatusCode = 200
        });
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("{streamGroupProfileId}/epg.xml")]
    [Route("{streamGroupProfileId}.xml")]
    //[Route("{streamGroupId}/{streamGroupProfileId}/epg.xml")]
    public async Task<IActionResult> GetStreamGroupEPG(int streamGroupProfileId)
    {
        //if (!streamGroupId.HasValue)
        //{
        //    streamGroupId = await streamGroupService.GetStreamGroupIdFromstreamGroupProfileIdAsync(streamGroupProfileId).ConfigureAwait(false);
        //}
        string xml = await Sender.Send(new GetStreamGroupEPG(streamGroupProfileId)).ConfigureAwait(false);
        return new FileContentResult(Encoding.UTF8.GetBytes(xml), "application/xml")
        {
            FileDownloadName = $"epg-{streamGroupProfileId}-{streamGroupProfileId}.xml"
        };
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("{streamGroupProfileId}/m3u.m3u")]
    [Route("{streamGroupProfileId}.m3u")]
    //[Route("{streamGroupId}/{streamGroupProfileId}/m3u.m3u")]
    public async Task<IActionResult> GetStreamGroupM3U(int streamGroupProfileId)
    {
        //if (!streamGroupId.HasValue)
        //{
        //    streamGroupId = await streamGroupService.GetStreamGroupIdFromstreamGroupProfileIdAsync(streamGroupProfileId).ConfigureAwait(false);
        //}
        string data = await Sender.Send(new GetStreamGroupM3U(streamGroupProfileId, true)).ConfigureAwait(false);

        return new FileContentResult(Encoding.UTF8.GetBytes(data), "application/x-mpegURL")
        {
            FileDownloadName = $"m3u-{streamGroupProfileId}-{streamGroupProfileId}.m3u"
        };
    }
}