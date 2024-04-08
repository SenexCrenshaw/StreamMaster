using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Domain.Authentication;
using StreamMaster.Domain.Configuration;

using System.Text.Json;
using System.Web;

namespace StreamMaster.Application.StreamGroups.QueriesOld;

public record GetStreamGroupVideoStreamUrl(string VideoStreamId) : IRequest<string?>;

internal class GetStreamGroupVideoStreamUrlHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupVideoStreamUrl> logger, IRepositoryWrapper Repository, IOptionsMonitor<HLSSettings> inthlssettings, IOptionsMonitor<Setting> intsettings)
    : IRequestHandler<GetStreamGroupVideoStreamUrl, string?>
{
    private readonly Setting settings = intsettings.CurrentValue;
    private readonly HLSSettings hlssettings = inthlssettings.CurrentValue;

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



        string url = httpContextAccessor.GetUrl();
        string videoUrl;

        if (hlssettings.HLSM3U8Enable)
        {
            videoUrl = $"{url}/api/stream/{videoStream.Id}.m3u8";
        }
        else
        {
            string encodedName = HttpUtility.HtmlEncode(videoStream.User_Tvg_name).Trim()
                    .Replace("/", "")
                    .Replace(" ", "_");

            string encodedNumbers = 0.EncodeValues128(request.VideoStreamId, settings.ServerKey);
            videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

        }

        string jsonString = JsonSerializer.Serialize(videoUrl);

        return jsonString;

    }
}
