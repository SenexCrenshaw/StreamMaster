using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Pagination;

using StreamMaster.Application.VideoStreamLinks;
using StreamMaster.Application.VideoStreamLinks.Commands;
using StreamMaster.Application.VideoStreamLinks.Queries;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IVideoStreamLinkHub
{
    public async Task AddVideoStreamToVideoStream(AddVideoStreamToVideoStreamRequest request)
    {
        await Sender.Send(request);
    }

    public async Task<List<string>> GetVideoStreamVideoStreamIds(GetVideoStreamVideoStreamIdsRequest request, CancellationToken cancellationToken)
    {
        return await Sender.Send(request, cancellationToken);
    }

    public async Task<PagedResponse<VideoStreamDto>> GetPagedVideoStreamVideoStreams(VideoStreamLinkParameters Parameters, CancellationToken cancellationToken)
    {
        return await Sender.Send(new GetPagedVideoStreamVideoStreams(Parameters), cancellationToken);
    }

    public async Task RemoveVideoStreamFromVideoStream(RemoveVideoStreamFromVideoStreamRequest request)
    {
        await Sender.Send(request);
    }


}