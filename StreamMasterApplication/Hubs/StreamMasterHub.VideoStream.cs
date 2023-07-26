using StreamMasterApplication.Common.Models;
using StreamMasterApplication.VideoStreams;
using StreamMasterApplication.VideoStreams.Commands;
using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IVideoStreamHub
{
    public async Task<VideoStreamDto?> AddVideoStream(AddVideoStreamRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<string?> DeleteVideoStream(DeleteVideoStreamRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<IEnumerable<ChannelLogoDto>> GetChannelLogoDtos()
    {
        return await _mediator.Send(new GetChannelLogoDtos()).ConfigureAwait(false);
    }

    public async Task<VideoStreamDto?> GetVideoStream(string id)
    {
        return await _mediator.Send(new GetVideoStream(id)).ConfigureAwait(false);
    }

    public async Task<IEnumerable<VideoStreamDto>> GetVideoStreams()
    {
        return await _mediator.Send(new GetVideoStreams()).ConfigureAwait(false);
    }

    public async Task<IEnumerable<ChannelNumberPair>> SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<VideoStreamDto?> UpdateVideoStream(UpdateVideoStreamRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<IEnumerable<VideoStreamDto>> UpdateVideoStreams(UpdateVideoStreamsRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }
}
