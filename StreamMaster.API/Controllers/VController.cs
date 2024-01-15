using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Application.StreamGroups.Queries;
using StreamMaster.Domain.Authentication;
using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Models;
using StreamMaster.Domain.Repository;
using StreamMaster.Infrastructure;

using System.Text;
using System.Web;

namespace StreamMaster.API.Controllers;

public class VController(IRepositoryWrapper Repository, ISender sender, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor) : Controller
{
    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [HttpHead]
    [Route("v/v/{shortId}")]
    [Route("v/v/{shortId}.ts")]
    public IActionResult GetVideoStreamStream(string shortId)
    {
        VideoStream? videoStream = Repository.VideoStream.FindByCondition(a => a.ShortId.ToLower() == shortId.ToLower()).FirstOrDefault();

        if (videoStream == null)
        {
            return new NotFoundResult();
        }

        string encodedName = HttpUtility.HtmlEncode(videoStream.User_Tvg_name).Trim()
        .Replace("/", "")
        .Replace(" ", "_");

        Setting setting = memoryCache.GetSetting();

        string url = httpContextAccessor.GetUrl();
        string encodedNumbers = 0.EncodeValues128(videoStream.Id, setting.ServerKey);
        string videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";
        return Redirect(videoUrl);
    }

    [Authorize(Policy = "SGLinks")]
    [HttpGet]
    [Route("v/s/{streamGroupName}")]
    [Route("v/s/{streamGroupName}.m3u")]
    [V1ApiController("[controller]")]
    public async Task<IActionResult> GetStreamGroupM3U(string streamGroupName)
    {

        List<StreamGroupDto> sgs = await Repository.StreamGroup.GetStreamGroups(CancellationToken.None);

        StreamGroupDto? sg = null;

        foreach (StreamGroupDto testSG in sgs)
        {
            string encodedName = HttpUtility.HtmlEncode(testSG.Name).Trim()
                    .Replace("/", "")
                    .Replace(" ", "_");

            if (encodedName.Equals(streamGroupName.Trim(), StringComparison.CurrentCultureIgnoreCase))
            {
                sg = testSG;
                break;
            }
        }

        if (sg == null)
        {
            return new NotFoundResult();
        }

        string data = await sender.Send(new GetStreamGroupM3U(sg.Id, true)).ConfigureAwait(false);

        return new FileContentResult(Encoding.UTF8.GetBytes(data), "application/x-mpegURL")
        {
            FileDownloadName = $"m3u-{streamGroupName}.m3u"
        };
    }
}