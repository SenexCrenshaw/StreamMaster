using StreamMasterApplication.Common.Models;
using StreamMasterApplication.VideoStreams;
using StreamMasterApplication.VideoStreams.Commands;
using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IVideoStreamHub
{
    public async Task AddVideoStream(AddVideoStreamRequest request)
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

    public async Task<IEnumerable<VideoStreamDto>> GetVideoStreams()
    {
        return await _mediator.Send(new GetVideoStreams()).ConfigureAwait(false);
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
}
