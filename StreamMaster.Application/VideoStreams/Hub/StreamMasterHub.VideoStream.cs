using StreamMaster.Application.VideoStreams;
using StreamMaster.Application.VideoStreams.Commands;
using StreamMaster.Application.VideoStreams.Queries;
using StreamMaster.Domain.Pagination;
using StreamMaster.Domain.Requests;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IVideoStreamHub
{
    public async Task CreateVideoStream(CreateVideoStreamRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteVideoStream(DeleteVideoStreamRequest request)
    {
        _ = await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task<VideoStreamDto?> GetVideoStream(string id)
    {
        return await Sender.Send(new GetVideoStream(id)).ConfigureAwait(false);
    }

    public async Task<PagedResponse<VideoStreamDto>> GetPagedVideoStreams(VideoStreamParameters Parameters)
    {
        return await Sender.Send(new GetPagedVideoStreams(Parameters)).ConfigureAwait(false);
    }

    public async Task ReSetVideoStreamsLogo(ReSetVideoStreamsLogoRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task SetVideoStreamSetEPGsFromName(SetVideoStreamSetEPGsFromNameRequest request)
    {
        _ = await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task SetVideoStreamsLogoFromEPG(SetVideoStreamsLogoFromEPGRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task UpdateVideoStream(UpdateVideoStreamRequest request)
    {
        _ = await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task UpdateVideoStreams(UpdateVideoStreamsRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task<IEnumerable<VideoStreamDto>> GetVideoStreamsByNamePattern(GetVideoStreamsByNamePatternQuery request)
    {
        return await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task<IEnumerable<string>> GetVideoStreamNamesByNamePattern(GetVideoStreamNamesByNamePatternQuery request)
    {
        return await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task UpdateAllVideoStreamsFromParameters(UpdateAllVideoStreamsFromParametersRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteAllVideoStreamsFromParameters(DeleteAllVideoStreamsFromParametersRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task SetVideoStreamChannelNumbersFromParameters(SetVideoStreamChannelNumbersFromParametersRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task SetVideoStreamsLogoFromEPGFromParameters(SetVideoStreamsLogoFromEPGFromParametersRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task ReSetVideoStreamsLogoFromParameters(ReSetVideoStreamsLogoFromParametersRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task AutoSetEPG(AutoSetEPGRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task AutoSetEPGFromParameters(AutoSetEPGFromParametersRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task SetVideoStreamTimeShifts(SetVideoStreamTimeShiftsRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task SetVideoStreamTimeShiftFromParameters(SetVideoStreamTimeShiftFromParametersRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task<List<IdName>> GetVideoStreamNames()
    {
        return await Sender.Send(new GetVideoStreamNamesRequest()).ConfigureAwait(false);
    }


    public async Task<VideoInfo> GetVideoStreamInfoFromId(string channelVideoStreamId)
    {
        return await Sender.Send(new GetVideoStreamInfoFromIdRequest(channelVideoStreamId)).ConfigureAwait(false);
    }

    public async Task<VideoInfo> GetVideoStreamInfoFromUrl(string channelVideoStreamId)
    {
        return await Sender.Send(new GetVideoStreamInfoFromUrlRequest(channelVideoStreamId)).ConfigureAwait(false);

    }

    public async Task<List<IdNameUrl>> GetVideoStreamNamesAndUrls()
    {
        return await Sender.Send(new GetVideoStreamNamesAndUrlsRequest()).ConfigureAwait(false);
    }
}