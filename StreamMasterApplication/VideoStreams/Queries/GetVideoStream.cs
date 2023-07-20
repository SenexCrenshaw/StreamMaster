using MediatR;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStream(int Id) : IRequest<VideoStreamDto?>;

internal class GetVideoStreamHandler : IRequestHandler<GetVideoStream, VideoStreamDto?>
{
    private readonly IAppDbContext _context;

    public GetVideoStreamHandler(

        IAppDbContext context
    )
    {
        _context = context;
    }

    public async Task<VideoStreamDto?> Handle(GetVideoStream request, CancellationToken cancellationToken)
    {
        return await _context.GetVideoStreamDto(request.Id, cancellationToken).ConfigureAwait(false);
    }
}
