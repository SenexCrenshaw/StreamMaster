using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.StreamGroups.Queries;
using StreamMaster.Application.StreamGroups.QueriesOld;

using System.Text;

namespace StreamMaster.API.Controllers;

[V1ApiController("s")]
public class SsController(ISender Sender, IStreamGroupService streamGroupService) : Controller
{

    [HttpGet]
    [AllowAnonymous]
    [Route("{sgProfileId}")]
    [Route("{sgProfileId}/capability")]
    [Route("{sgProfileId}/device.xml")]
    public async Task<IActionResult> GetStreamGroupCapability(int sgProfileId)
    {
        string xml = await Sender.Send(new GetStreamGroupCapability(sgProfileId)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = xml,
            ContentType = "application/xml",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Authorize(Policy = "SGLinks")]
    [Route("{sgProfileId}/discover.json")]
    public async Task<IActionResult> GetStreamGroupDiscover(int sgProfileId)
    {
        int streamGroupId = await streamGroupService.GetStreamGroupIdFromSGProfileIdAsync(sgProfileId).ConfigureAwait(false);
        string json = await Sender.Send(new GetStreamGroupDiscover(streamGroupId)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = json,
            ContentType = "text/json",
            StatusCode = 200
        };
    }


    [HttpGet]
    [Authorize(Policy = "SGLinks")]
    [Route("{sgProfileId}/lineup.json")]
    public async Task<IActionResult> GetStreamGroupLineup(int sgProfileId)
    {
        string json = await Sender.Send(new GetStreamGroupLineup(sgProfileId, true)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = json,
            ContentType = "application/json",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Authorize(Policy = "SGLinks")]
    [Route("{sgProfileId}/lineup_status.json")]
    public async Task<IActionResult> GetStreamGroupLineupStatus(int sgProfileId)
    {
        int streamGroupId = await streamGroupService.GetStreamGroupIdFromSGProfileIdAsync(sgProfileId).ConfigureAwait(false);
        string json = await Sender.Send(new GetStreamGroupLineupStatus()).ConfigureAwait(false);
        return new ContentResult
        {
            Content = json,
            ContentType = "text/json",
            StatusCode = 200
        };
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("{sgProfileId}/epg.xml")]
    //[Route("{streamGroupId}/{sgProfileId}/epg.xml")]
    public async Task<IActionResult> GetStreamGroupEPG(int sgProfileId)
    {
        //if (!streamGroupId.HasValue)
        //{
        //    streamGroupId = await streamGroupService.GetStreamGroupIdFromSGProfileIdAsync(sgProfileId).ConfigureAwait(false);
        //}
        string xml = await Sender.Send(new GetStreamGroupEPG(sgProfileId)).ConfigureAwait(false);
        return new FileContentResult(Encoding.UTF8.GetBytes(xml), "application/xml")
        {
            FileDownloadName = $"epg-{sgProfileId}-{sgProfileId}.xml"
        };
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("{sgProfileId}/m3u.m3u")]
    //[Route("{streamGroupId}/{sgProfileId}/m3u.m3u")]
    public async Task<IActionResult> GetStreamGroupM3U(int sgProfileId)
    {
        //if (!streamGroupId.HasValue)
        //{
        //    streamGroupId = await streamGroupService.GetStreamGroupIdFromSGProfileIdAsync(sgProfileId).ConfigureAwait(false);
        //}
        string data = await Sender.Send(new GetStreamGroupM3U(sgProfileId, true)).ConfigureAwait(false);

        return new FileContentResult(Encoding.UTF8.GetBytes(data), "application/x-mpegURL")
        {
            FileDownloadName = $"m3u-{sgProfileId}-{sgProfileId}.m3u"
        };
    }
}