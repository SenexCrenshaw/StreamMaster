using Microsoft.AspNetCore.Http;

using System.Text.Json;
using System.Web;

namespace StreamMaster.Application.SMChannels;

public partial class SMChannelsService(IRepositoryWrapper repository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<HLSSettings> inthlssettings, IOptionsMonitor<Setting> intsettings)
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

        foreach (SMChannelDto channel in res.Data)
        {
            List<SMChannelStreamLink> links = repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == channel.Id).ToList();

            string videoUrl;
            foreach (SMStreamDto stream in channel.SMStreams)
            {
                SMChannelStreamLink? link = links.FirstOrDefault(a => a.SMStreamId == stream.Id);

                if (link != null)
                {
                    stream.Rank = link.Rank;
                }
            }

            if (hlssettings.HLSM3U8Enable)
            {
                videoUrl = $"{url}/api/stream/{channel.Id}.m3u8";
            }
            else
            {
                string encodedName = HttpUtility.HtmlEncode(channel.Name).Trim()
                        .Replace("/", "")
                        .Replace(" ", "_");

                string encodedNumbers = 0.EncodeValues128(channel.Id, settings.ServerKey);
                videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

            }

            string jsonString = JsonSerializer.Serialize(videoUrl);
            channel.RealUrl = jsonString;
        }

        ret.PagedResponse = res;
        return ret;
    }

    [SMAPI]
    public async Task<DefaultAPIResponse> CreateSMChannelFromStream(string streamId)
    {
        await repository.SMChannel.CreateSMChannelFromStream(streamId);

        await hubContext.Clients.All.DataRefresh("SMChannelDto").ConfigureAwait(false);

        return APIResponseFactory.Ok();
    }

    [SMAPI]
    public async Task<DefaultAPIResponse> DeleteSMChannels(List<int> smchannelIds)
    {
        DefaultAPIResponse ret = await repository.SMChannel.DeleteSMChannels(smchannelIds);
        if (!ret.IsError.HasValue)
        {
            await hubContext.Clients.All.DataRefresh("SMChannelDto").ConfigureAwait(false);
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

        await hubContext.Clients.All.DataRefresh("SMChannelDto").ConfigureAwait(false);

        return APIResponseFactory.Ok();
    }

    [SMAPI]
    public async Task<DefaultAPIResponse> DeleteAllSMChannelsFromParameters(SMChannelParameters Parameters)
    {
        List<int> ids = await repository.SMChannel.DeleteAllSMChannelsFromParameters(Parameters).ConfigureAwait(false);

        if (ids.Count != 0)
        {
            await hubContext.Clients.All.DataRefresh("SMChannelDto").ConfigureAwait(false);
        }

        return APIResponseFactory.Ok();
    }


    [SMAPI]
    public async Task<DefaultAPIResponse> AddSMStreamToSMChannel(SMStreamSMChannelRequest request)
    {
        DefaultAPIResponse ret = await repository.SMChannel.AddSMStreamToSMChannel(request.SMChannelId, request.SMStreamId).ConfigureAwait(false);
        if (!ret.IsError.HasValue)
        {
            SMChannel? channel = repository.SMChannel.GetSMChannel(request.SMChannelId);
            if (channel != null)
            {
                List<SMStreamDto> streams = UpdateStreamRanks(channel.Id, channel.SMStreams.Select(a => a.SMStream).ToList());
                FieldData fd = new(nameof(SMChannelDto), channel.Id.ToString(), "smStreams", streams);
                await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
            }
        }
        return ret;
    }

    [SMAPI]
    public async Task<DefaultAPIResponse> RemoveSMStreamFromSMChannel(SMStreamSMChannelRequest request)
    {
        DefaultAPIResponse ret = await repository.SMChannel.RemoveSMStreamFromSMChannel(request.SMChannelId, request.SMStreamId).ConfigureAwait(false);
        if (!ret.IsError.HasValue)
        {
            SMChannel? channel = repository.SMChannel.GetSMChannel(request.SMChannelId);
            if (channel != null)
            {
                List<SMStreamDto> streams = UpdateStreamRanks(channel.Id, channel.SMStreams.Select(a => a.SMStream).ToList());
                FieldData fd = new(nameof(SMChannelDto), channel.Id.ToString(), "smStreams", streams);

                await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
            }
        }
        return ret;
    }

    [SMAPI]
    public async Task<DefaultAPIResponse> SetSMStreamRanks(List<SMChannelRankRequest> requests)
    {
        DefaultAPIResponse ret = await repository.SMChannel.SetSMStreamRanks(requests).ConfigureAwait(false);
        if (!ret.IsError.HasValue)
        {
            List<FieldData> fieldDatas = [];
            foreach (int smChannelId in requests.Select(a => a.SMChannelId).Distinct())
            {
                SMChannel? channel = repository.SMChannel.GetSMChannel(smChannelId);
                if (channel != null)
                {
                    List<SMStreamDto> streams = UpdateStreamRanks(channel.Id, channel.SMStreams.Select(a => a.SMStream).ToList());
                    FieldData fd = new(nameof(SMChannelDto), channel.Id.ToString(), "smStreams", streams);
                    fieldDatas.Add(fd);
                }

            }
            await hubContext.Clients.All.SetField(fieldDatas.ToArray()).ConfigureAwait(false);
        }
        return ret;
    }
    private List<SMStreamDto> UpdateStreamRanks(int SMChannelId, List<SMStream> streams)
    {

        List<SMStreamDto> ret = [];

        List<SMChannelStreamLink> links = [.. repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == SMChannelId)];

        foreach (SMStream stream in streams)
        {
            SMChannelStreamLink? link = links.FirstOrDefault(a => a.SMStreamId == stream.Id);

            if (link != null)
            {
                SMStreamDto sm = mapper.Map<SMStreamDto>(stream);
                sm.Rank = link.Rank;
                ret.Add(sm);
            }
        }
        return ret;
    }

}
