using StreamMasterApplication.VideoStreams;
using StreamMasterApplication.VideoStreams.Commands;
using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IVideoStreamHub
{

    public async Task<PagedResponse<VideoStreamDto>> GetVideoStreamsForChannelGroups(VideoStreamParameters videoStreamParameters)
    {
        PagedResponse<VideoStreamDto> ret = await _mediator.Send(new GetVideoStreamsForChannelGroups(videoStreamParameters)).ConfigureAwait(false);
        return ret;
    }
    public async Task CreateVideoStream(CreateVideoStreamRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteVideoStream(DeleteVideoStreamRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<IEnumerable<ChannelLogoDto>> GetChannelLogoDtos()
    {
        return await _mediator.Send(new GetChannelLogoDtos()).ConfigureAwait(false);
    }

    public async Task<VideoStreamDto?> GetVideoStream(string id)
    {
        return await _mediator.Send(new GetVideoStream(id)).ConfigureAwait(false);
    }

    public async Task<PagedResponse<VideoStreamDto>> GetVideoStreams(VideoStreamParameters Parameters)
    {
        return await _mediator.Send(new GetVideoStreams(Parameters)).ConfigureAwait(false);
    }

    public async Task ReSetVideoStreamsLogo(ReSetVideoStreamsLogoRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task SetVideoStreamSetEPGsFromName(SetVideoStreamSetEPGsFromNameRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task SetVideoStreamsLogoToEPG(SetVideoStreamsLogoToEPGRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task UpdateVideoStream(UpdateVideoStreamRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task UpdateVideoStreams(UpdateVideoStreamsRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<IEnumerable<VideoStreamDto>> GetVideoStreamsByNamePattern(GetVideoStreamsByNamePatternQuery request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<IEnumerable<string>> GetVideoStreamNamesByNamePattern(GetVideoStreamNamesByNamePatternQuery request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task UpdateAllVideoStreamsFromParameters(UpdateAllVideoStreamsFromParametersRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteAllVideoStreamsFromParameters(DeleteAllVideoStreamsFromParametersRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task SetVideoStreamChannelNumbersFromParameters(SetVideoStreamChannelNumbersFromParametersRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }
}