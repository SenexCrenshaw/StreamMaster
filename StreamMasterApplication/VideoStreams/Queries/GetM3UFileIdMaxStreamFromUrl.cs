using MediatR;

namespace StreamMasterApplication.VideoStreams.Queries;

public class M3UFileIdMaxStream
{
    public int M3UFileId { get; set; }
    public int MaxStreams { get; set; }
}

public record GetM3UFileIdMaxStreamFromUrl(string Url) : IRequest<M3UFileIdMaxStream?>;

internal class GetM3UFileIdMaxStreamFromUrlHandler : IRequestHandler<GetM3UFileIdMaxStreamFromUrl, M3UFileIdMaxStream?>
{
    private readonly IAppDbContext _context;

    public GetM3UFileIdMaxStreamFromUrlHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<M3UFileIdMaxStream?> Handle(GetM3UFileIdMaxStreamFromUrl request, CancellationToken cancellationToken)
    {
        var videoStream = _context.VideoStreams.FirstOrDefault(a => a.User_Url == request.Url);

        if (videoStream == null)
        {
            return null;
        }

        if (videoStream.M3UFileId == 0)
        {
            return new M3UFileIdMaxStream { M3UFileId = videoStream.M3UFileId, MaxStreams = 999 };
        }

        var m3uFile = _context.M3UFiles.Single(a => a.Id == videoStream.M3UFileId);

        return new M3UFileIdMaxStream { M3UFileId = videoStream.M3UFileId, MaxStreams = m3uFile.MaxStreamCount };
    }
}
