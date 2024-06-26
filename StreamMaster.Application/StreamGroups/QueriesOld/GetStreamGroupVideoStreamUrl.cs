using Microsoft.AspNetCore.Http;

using System.Text.Json;
using System.Web;

namespace StreamMaster.Application.StreamGroups.QueriesOld;

public record GetStreamGroupVideoStreamUrl(int smChannelId) : IRequest<string?>;

internal class GetStreamGroupVideoStreamUrlHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupVideoStreamUrl> logger, IRepositoryWrapper Repository, IOptionsMonitor<HLSSettings> inthlssettings, IOptionsMonitor<Setting> intsettings)
    : IRequestHandler<GetStreamGroupVideoStreamUrl, string?>
{
    private readonly Setting settings = intsettings.CurrentValue;
    private readonly HLSSettings hlssettings = inthlssettings.CurrentValue;

    public async Task<string?> Handle(GetStreamGroupVideoStreamUrl request, CancellationToken cancellationToken = default)
    {
        if (request.smChannelId < 1)
        {
            return null;
        }
        SMChannel? videoStream = await Repository.SMChannel.FirstOrDefaultAsync(a => a.Id == request.smChannelId).ConfigureAwait(false);
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
            string encodedName = HttpUtility.HtmlEncode(videoStream.Name).Trim()
                    .Replace("/", "")
                    .Replace(" ", "_");

            string encodedNumbers = 0.EncodeValues128(request.smChannelId, settings.ServerKey);
            videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

        }

        string jsonString = JsonSerializer.Serialize(videoUrl);

        return jsonString;

    }
}
