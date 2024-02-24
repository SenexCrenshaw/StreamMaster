using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Domain.Authentication;

using System.Web;

namespace StreamMaster.Application.VideoStreams.Queries;

public record GetVideoStreamNamesAndUrlsRequest() : IRequest<List<IdNameUrl>>;

internal class GetVideoStreamNamesAndUrlsHandler(ILogger<GetVideoStreamNamesAndUrlsRequest> logger, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor, IRepositoryWrapper Repository) : IRequestHandler<GetVideoStreamNamesAndUrlsRequest, List<IdNameUrl>>
{
    public async Task<List<IdNameUrl>> Handle(GetVideoStreamNamesAndUrlsRequest request, CancellationToken cancellationToken)
    {
        string url = httpContextAccessor.GetUrl();
        Setting setting = memoryCache.GetSetting();

        List<IdNameUrl> matchedIds = await Repository.VideoStream.GetVideoStreamQuery()
            .Where(vs => !vs.IsHidden)
            .OrderBy(vs => vs.User_Tvg_name)
            .Select(vs => new IdNameUrl(vs.Id, vs.User_Tvg_name, GetVideoStreamUrl(vs, setting, url)))
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