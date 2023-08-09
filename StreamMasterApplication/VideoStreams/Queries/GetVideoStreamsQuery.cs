using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreamsQuery(VideoStreamParameters Parameters) : IRequest<PagedResponse<VideoStream>>;

internal class GetVideoStreamsQueryHandler : BaseRequestHandler, IRequestHandler<GetVideoStreamsQuery, PagedResponse<VideoStream>>
{
    public GetVideoStreamsQueryHandler(ILogger<ChangeM3UFileNameRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public async Task<PagedResponse<VideoStream>> Handle(GetVideoStreamsQuery request, CancellationToken cancellationToken)
    {
        return await Repository.VideoStream.GetVideoStreamsAsync(request.Parameters, cancellationToken).ConfigureAwait(false);
    }
}