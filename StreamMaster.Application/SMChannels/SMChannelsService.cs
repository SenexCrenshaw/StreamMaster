using Microsoft.AspNetCore.Http;

using System.Text.Json;
using System.Web;

namespace StreamMaster.Application.SMChannels;

public partial class SMChannelsService(IRepositoryWrapper repository, IHttpContextAccessor httpContextAccessor, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<HLSSettings> inthlssettings, IOptionsMonitor<Setting> intsettings)
    : ISMChannelsService

{
    private readonly Setting settings = intsettings.CurrentValue;
    private readonly HLSSettings hlssettings = inthlssettings.CurrentValue;

    [SMAPI]
    public async Task<APIResponse<SMChannelDto>> GetPagedSMChannels(SMChannelParameters Parameters)
    {
        APIResponse<SMChannelDto> ret = new();
        if (Parameters.PageSize == 0)
        {
            ret.PagedResponse = repository.SMChannel.CreateEmptyPagedResponse();
            return ret;
        }

        PagedResponse<SMChannelDto> res = await repository.SMChannel.GetPagedSMChannels(Parameters).ConfigureAwait(false);

        string url = httpContextAccessor.GetUrl();
        foreach (SMChannelDto stream in res.Data)
        {
            string videoUrl;

            if (hlssettings.HLSM3U8Enable)
            {
                videoUrl = $"{url}/api/stream/{stream.Id}.m3u8";
            }
            else
            {
                string encodedName = HttpUtility.HtmlEncode(stream.Name).Trim()
                        .Replace("/", "")
                        .Replace(" ", "_");

                string encodedNumbers = 0.EncodeValues128(stream.Id, settings.ServerKey);
                videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

            }

            string jsonString = JsonSerializer.Serialize(videoUrl);
            stream.RealUrl = jsonString;
        }

        ret.PagedResponse = res;
        return ret;
    }

    [SMAPI]
    public async Task<DefaultAPIResponse> CreateSMChannelFromStream(string streamId)
    {
        await repository.SMChannel.CreateSMChannelFromStream(streamId);

        await hubContext.Clients.All.DataRefresh("SMStreamDto").ConfigureAwait(false);

        return APIResponseFactory.Ok();
    }

    [SMAPI]
    public async Task<DefaultAPIResponse> DeleteSMChannels(List<int> smchannelIds)
    {
        DefaultAPIResponse ret = await repository.SMChannel.DeleteSMChannels(smchannelIds);
        if (!ret.IsError.HasValue)
        {
            await hubContext.Clients.All.DataRefresh("SMStreamDto").ConfigureAwait(false);
        }

        return ret;
    }

    [SMAPI]
    public async Task<DefaultAPIResponse> DeleteSMChannel(int smchannelId)
    {
        SMChannel? channel = repository.SMChannel.GetSMChannel(smchannelId);
        if (channel == null)
        {
            return APIResponseFactory.NotFound();
        }

        await repository.SMChannel.DeleteSMChannel(smchannelId);

        await hubContext.Clients.All.DataRefresh("SMStreamDto").ConfigureAwait(false);

        return APIResponseFactory.Ok();
    }

    [SMAPI]
    public async Task<DefaultAPIResponse> DeleteAllSMChannelsFromParameters(SMChannelParameters Parameters)
    {
        List<int> ids = await repository.SMChannel.DeleteAllSMChannelsFromParameters(Parameters).ConfigureAwait(false);

        if (ids.Count != 0)
        {
            await hubContext.Clients.All.DataRefresh("SMStreamDto").ConfigureAwait(false);
        }

        return APIResponseFactory.Ok();
    }
}
