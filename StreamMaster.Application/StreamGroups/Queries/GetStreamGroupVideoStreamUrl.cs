using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Domain.Authentication;

using System.Text.Json;
using System.Web;

namespace StreamMaster.Application.StreamGroups.Queries;

public record GetStreamGroupVideoStreamUrl(string VideoStreamId) : IRequest<string?>;

internal class GetStreamGroupVideoStreamUrlHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupVideoStreamUrl> logger, IRepositoryWrapper Repository, IMemoryCache memoryCache)
    : IRequestHandler<GetStreamGroupVideoStreamUrl, string?>
{
    public async Task<string?> Handle(GetStreamGroupVideoStreamUrl request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.VideoStreamId))
        {
            return null;
        }
        VideoStreamDto? videoStream = await Repository.VideoStream.GetVideoStreamById(request.VideoStreamId).ConfigureAwait(false);
        if (videoStream == null)
        {
            return null;
        }

        Setting setting = memoryCache.GetSetting();

        string url = httpContextAccessor.GetUrl();
        string videoUrl;

        if (setting.HLS.HLSM3U8Enable)
        {
            videoUrl = $"{url}/api/stream/{videoStream.Id}.m3u8";
        }
        else
        {
            string encodedName = HttpUtility.HtmlEncode(videoStream.User_Tvg_name).Trim()
                    .Replace("/", "")
                    .Replace(" ", "_");

            string encodedNumbers = 0.EncodeValues128(request.VideoStreamId, setting.ServerKey);
            videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

        }

        string jsonString = JsonSerializer.Serialize(videoUrl);

        return jsonString;

    }
}
