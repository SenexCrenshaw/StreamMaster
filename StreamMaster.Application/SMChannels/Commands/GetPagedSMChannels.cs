using Microsoft.AspNetCore.Http;

using System.Text.Json;
using System.Web;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
public record GetPagedSMChannels(SMChannelParameters Parameters) : IRequest<APIResponse<SMChannelDto>>;

internal class GetPagedSMChannelsRequestHandler(IRepositoryWrapper Repository, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetPagedSMChannels, APIResponse<SMChannelDto>>
{
    public async Task<APIResponse<SMChannelDto>> Handle(GetPagedSMChannels request, CancellationToken cancellationToken)
    {
        APIResponse<SMChannelDto> ret = new();
        if (request.Parameters.PageSize == 0)
        {
            ret.PagedResponse = Repository.SMChannel.CreateEmptyPagedResponse();
            return ret;
        }

        PagedResponse<SMChannelDto> res = await Repository.SMChannel.GetPagedSMChannels(request.Parameters).ConfigureAwait(false);

        string url = httpContextAccessor.GetUrl();

        foreach (SMChannelDto channel in res.Data)
        {
            List<SMChannelStreamLink> links = Repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == channel.Id).ToList();

            string videoUrl;
            foreach (SMStreamDto stream in channel.SMStreams)
            {
                SMChannelStreamLink? link = links.FirstOrDefault(a => a.SMStreamId == stream.Id);

                if (link != null)
                {
                    stream.Rank = link.Rank;
                }
            }

            if (hlsSettings.CurrentValue.HLSM3U8Enable)
            {
                videoUrl = $"{url}/api/stream/{channel.Id}.m3u8";
            }
            else
            {
                string encodedName = HttpUtility.HtmlEncode(channel.Name).Trim()
                        .Replace("/", "")
                        .Replace(" ", "_");

                string encodedNumbers = 0.EncodeValues128(channel.Id, settings.CurrentValue.ServerKey);
                videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

            }

            string jsonString = JsonSerializer.Serialize(videoUrl);
            channel.RealUrl = jsonString;
        }

        ret.PagedResponse = res;
        return ret;
    }
}
