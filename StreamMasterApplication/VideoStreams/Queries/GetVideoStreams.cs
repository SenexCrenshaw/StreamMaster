using MediatR;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreams : IRequest<IEnumerable<VideoStreamDto>>;

internal class GetVideoStreamsHandler : IRequestHandler<GetVideoStreams, IEnumerable<VideoStreamDto>>
{
    private readonly IAppDbContext _context;

    public GetVideoStreamsHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VideoStreamDto>> Handle(GetVideoStreams request, CancellationToken cancellationToken)
    {
        return await _context.GetVideoStreamsDto(cancellationToken).ConfigureAwait(false);
    }
}
