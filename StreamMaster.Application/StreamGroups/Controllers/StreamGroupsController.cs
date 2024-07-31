using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Crypto.Commands;
using StreamMaster.Application.StreamGroups.Queries;
using StreamMaster.Application.StreamGroups.QueriesOld;

using System.Text;

namespace StreamMaster.Application.StreamGroups.Controllers;

public partial class StreamGroupsController
{

    [HttpGet]
    [AllowAnonymous]
    [Route("{encodedId}")]
    [Route("{encodedId}/capability")]
    [Route("{encodedId}/device.xml")]
    public async Task<IActionResult> GetStreamGroupCapability(string encodedId)
    {
        (int? streamGroupId, int? streamGroupProfileId) = await Sender.Send(new DecodeProfileIdFromEncoded(encodedId)).ConfigureAwait(false);
        if (!streamGroupId.HasValue || !streamGroupProfileId.HasValue)
        {
            return new NotFoundResult();
        }

        string xml = await Sender.Send(new GetStreamGroupCapability(streamGroupId.Value, streamGroupProfileId.Value)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = xml,
            ContentType = "application/xml",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Authorize(Policy = "SGLinks")]
    [Route("{encodedId}/discover.json")]
    public async Task<IActionResult> GetStreamGroupDiscover(string encodedId)
    {
        (int? streamGroupId, int? streamGroupProfileId) = await Sender.Send(new DecodeProfileIdFromEncoded(encodedId)).ConfigureAwait(false);
        if (!streamGroupId.HasValue || !streamGroupProfileId.HasValue)
        {
            return new NotFoundResult();
        }

        string json = await Sender.Send(new GetStreamGroupDiscover(streamGroupId.Value, streamGroupProfileId.Value)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = json,
            ContentType = "text/json",
            StatusCode = 200
        };
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("{encodedId}/epg.xml")]
    public async Task<IActionResult> GetStreamGroupEPG(string encodedId)
    {
        (int? streamGroupId, int? streamGroupProfileId) = await Sender.Send(new DecodeProfileIdFromEncoded(encodedId)).ConfigureAwait(false);
        if (!streamGroupId.HasValue || !streamGroupProfileId.HasValue)
        {
            return new NotFoundResult();
        }

        StreamGroup? sg = await StreamGroupService.GetStreamGroupFromIdAsync(streamGroupId.Value).ConfigureAwait(false);
        if (sg == null)
        {
            return new NotFoundResult();
        }

        string xml = await Sender.Send(new GetStreamGroupEPG(streamGroupId.Value, streamGroupProfileId.Value)).ConfigureAwait(false);
        return new FileContentResult(Encoding.UTF8.GetBytes(xml), "application/xml")
        {
            FileDownloadName = $"epg-{sg.Name}.xml"
        };
    }


    [HttpGet]
    [Authorize(Policy = "SGLinks")]
    [Route("{encodedId}/lineup.json")]
    public async Task<IActionResult> GetStreamGroupLineup(string encodedId)
    {
        (int? streamGroupId, int? streamGroupProfileId) = await Sender.Send(new DecodeProfileIdFromEncoded(encodedId)).ConfigureAwait(false);
        if (!streamGroupId.HasValue || !streamGroupProfileId.HasValue)
        {
            return new NotFoundResult();
        }

        string json = await Sender.Send(new GetStreamGroupLineup(streamGroupId.Value, streamGroupProfileId.Value)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = json,
            ContentType = "application/json",
            StatusCode = 200
        };
    }

    [HttpGet]
    [Authorize(Policy = "SGLinks")]
    [Route("{encodedId}/lineup_status.json")]
    public async Task<IActionResult> GetStreamGroupLineupStatus(string encodedId)
    {
        (int? streamGroupId, int? streamGroupProfileId) = await Sender.Send(new DecodeProfileIdFromEncoded(encodedId)).ConfigureAwait(false);
        if (!streamGroupId.HasValue || !streamGroupProfileId.HasValue)
        {
            return new NotFoundResult();
        }

        string json = await Sender.Send(new GetStreamGroupLineupStatus(streamGroupId.Value, streamGroupProfileId.Value)).ConfigureAwait(false);
        return new ContentResult
        {
            Content = json,
            ContentType = "text/json",
            StatusCode = 200
        };
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("{encodedId}/m3u.m3u")]
    public async Task<IActionResult> GetStreamGroupM3U(string encodedId)
    {
        (int? streamGroupId, int? streamGroupProfileId) = await Sender.Send(new DecodeProfileIdFromEncoded(encodedId)).ConfigureAwait(false);
        if (!streamGroupId.HasValue || !streamGroupProfileId.HasValue)
        {
            return new NotFoundResult();
        }

        string data = await Sender.Send(new GetStreamGroupM3U(streamGroupId.Value, streamGroupProfileId.Value)).ConfigureAwait(false);

        return new FileContentResult(Encoding.UTF8.GetBytes(data), "application/x-mpegURL")
        {
            FileDownloadName = $"m3u-{streamGroupId.Value}.m3u"
        };
    }

}