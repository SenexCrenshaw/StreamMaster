using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreams(VideoStreamParameters Parameters) : IRequest<IPagedList<VideoStream>>;

internal class GetVideoStreamsHandler : BaseRequestHandler, IRequestHandler<GetVideoStreams, IPagedList<VideoStream>>
{
    public GetVideoStreamsHandler(ILogger<ChangeM3UFileNameRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public async Task<IPagedList<VideoStream>> Handle(GetVideoStreams request, CancellationToken cancellationToken)
    {
        return await Repository.VideoStream.GetVideoStreamsAsync(request.Parameters, cancellationToken).ConfigureAwait(false);
    }
}