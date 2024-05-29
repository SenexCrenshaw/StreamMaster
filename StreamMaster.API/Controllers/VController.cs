using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Application.StreamGroups.QueriesOld;
using StreamMaster.Domain.Authentication;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Requests;

using System.Text;
using System.Web;

namespace StreamMaster.API.Controllers;


public class VController(IRepositoryWrapper Repository, ISender sender, IOptionsMonitor<HLSSettings> inthlssettings, IOptionsMonitor<Setting> intsettings, IHttpContextAccessor httpContextAccessor) : Controller
{
    private readonly Setting settings = intsettings.CurrentValue;
    private readonly HLSSettings hlssettings = inthlssettings.CurrentValue;

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [HttpHead]
    [Route("v/v/{SMChannelId}")]
    [Route("v/v/{SMChannelId}.ts")]
    public IActionResult GetVideoStreamStream(string SMChannelId)
    {
        SMChannel? smChannel = Repository.SMChannel.FirstOrDefault(a => a.SMChannelId == SMChannelId);

        if (smChannel == null)
        {
            return new NotFoundResult();
        }


        string videoUrl;
        string url = httpContextAccessor.GetUrl();
        if (hlssettings.HLSM3U8Enable)
        {
            videoUrl = $"{url}/api/stream/{smChannel.Id}.m3u8";
            return Redirect(videoUrl);
        }

        string encodedName = HttpUtility.HtmlEncode(smChannel.Name).Trim()
        .Replace("/", "")
        .Replace(" ", "_");

        string encodedNumbers = 0.EncodeValues128(smChannel.Id, settings.ServerKey);
        videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

        return Redirect(videoUrl);
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("v/s/{streamGroupName}.m3u")]

    public async Task<IActionResult> GetStreamGroupM3U(string streamGroupName)
    {

        StreamGroupDto? sg = await GetStreamGroupDto(streamGroupName);

        if (sg == null)
        {
            return new NotFoundResult();
        }

        string encodedName = GetEncodedName(streamGroupName);
        string data = await sender.Send(new GetStreamGroupM3U(sg.Id, true)).ConfigureAwait(false);

        return new FileContentResult(Encoding.UTF8.GetBytes(data), "application/x-mpegURL")
        {
            FileDownloadName = $"m3u-{encodedName}.m3u"
        };
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("v/s/{streamGroupName}.xml")]
    public async Task<IActionResult> GetStreamGroupEPG(string streamGroupName)
    {
        StreamGroupDto? sg = await GetStreamGroupDto(streamGroupName);

        if (sg == null)
        {
            return new NotFoundResult();
        }

        string encodedName = GetEncodedName(streamGroupName);
        string xml = await sender.Send(new GetStreamGroupEPG(sg.Id)).ConfigureAwait(false);
        return new FileContentResult(Encoding.UTF8.GetBytes(xml), "application/xml")
        {
            FileDownloadName = $"epg-{encodedName}.xml"
        };
    }

    private static string GetEncodedName(string name)
    {
        return HttpUtility.HtmlEncode(name).Trim()
                            .Replace("/", "")
                            .Replace(" ", "_");
    }

    private async Task<StreamGroupDto?> GetStreamGroupDto(string streamGroupName)
    {
        List<StreamGroupDto> sgs = await Repository.StreamGroup.GetStreamGroups(CancellationToken.None);

        foreach (StreamGroupDto testSG in sgs)
        {
            string encodedName = GetEncodedName(testSG.Name);

            if (encodedName.Equals(streamGroupName.Trim(), StringComparison.CurrentCultureIgnoreCase))
            {
                return testSG;
            }
        }

        return null;

    }
}