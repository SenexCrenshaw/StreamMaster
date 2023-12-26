using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Authentication;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.Common.Extensions;

using System.Text.Json;
using System.Web;

namespace StreamMaster.Application.StreamGroups.Queries;

public record GetStreamGroupVideoStreamUrl(string VideoStreamId) : IRequest<string?>;

internal class GetStreamGroupVideoStreamUrlHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupVideoStreamUrl> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetStreamGroupVideoStreamUrl, string?>
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

        Setting setting = await GetSettingsAsync();

        string encodedName = HttpUtility.HtmlEncode(videoStream.User_Tvg_name).Trim()
                .Replace("/", "")
                .Replace(" ", "_");

        string encodedNumbers = 0.EncodeValues128(request.VideoStreamId, setting.ServerKey);
        string url = httpContextAccessor.GetUrl();
        string videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

        string jsonString = JsonSerializer.Serialize(videoUrl);

        return jsonString;

    }
}
