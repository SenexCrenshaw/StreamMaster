using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Domain.Authentication;
using StreamMaster.Domain.Configuration;
using System.Web;

namespace StreamMaster.Application.VideoStreams.Queries;

public record GetVideoStreamNamesAndUrlsRequest() : IRequest<List<IdNameUrl>>;

internal class GetVideoStreamNamesAndUrlsHandler(ILogger<GetVideoStreamNamesAndUrlsRequest> logger, IOptionsMonitor<Setting> intsettings, IHttpContextAccessor httpContextAccessor, IRepositoryWrapper Repository) : IRequestHandler<GetVideoStreamNamesAndUrlsRequest, List<IdNameUrl>>
{
    private readonly Setting settings = intsettings.CurrentValue;

    public async Task<List<IdNameUrl>> Handle(GetVideoStreamNamesAndUrlsRequest request, CancellationToken cancellationToken)
    {
        string url = httpContextAccessor.GetUrl();


        List<IdNameUrl> matchedIds = await Repository.VideoStream.GetVideoStreamQuery()
            .Where(vs => !vs.IsHidden)
            .OrderBy(vs => vs.User_Tvg_name)
            .Select(vs => new IdNameUrl(vs.Id, vs.User_Tvg_name, GetVideoStreamUrl(vs, settings, url)))
            .ToListAsync(cancellationToken: cancellationToken);


        return matchedIds;

    }
    private static string GetVideoStreamUrl(VideoStream videoStream, Setting setting, string url)
    {

        string videoUrl;

        if (setting.HLS.HLSM3U8Enable)
        {
            videoUrl = $"{url}/api/stream/{videoStream.Id}.m3u8";
            return videoUrl;
        }

        string encodedName = HttpUtility.HtmlEncode(videoStream.User_Tvg_name).Trim()
        .Replace("/", "")
        .Replace(" ", "_");

        string encodedNumbers = 0.EncodeValues128(videoStream.Id, setting.ServerKey);
        videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

        return videoUrl;
    }
}