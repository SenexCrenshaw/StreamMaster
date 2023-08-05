using StreamMasterApplication.VideoStreams;
using StreamMasterApplication.VideoStreams.Commands;
using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IVideoStreamHub
{
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

    public async Task<PagedList<VideoStream>> GetVideoStreams(VideoStreamParameters Parameters)
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

    public async Task<IEnumerable<VideoStream>> GetVideoStreamsByNamePattern(GetVideoStreamsByNamePatternQuery request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }
}
