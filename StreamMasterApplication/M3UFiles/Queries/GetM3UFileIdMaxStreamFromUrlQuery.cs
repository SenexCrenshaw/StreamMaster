using MediatR;

namespace StreamMasterApplication.M3UFiles.Queries;

public record GetM3UFileIdMaxStreamFromUrlQuery(string Url) : IRequest<M3UFileIdMaxStream?>;

internal class GetM3UFileIdMaxStreamFromUrlQueryHandler : IRequestHandler<GetM3UFileIdMaxStreamFromUrlQuery, M3UFileIdMaxStream?>
{
    private IRepositoryWrapper Repository { get; }
    public GetM3UFileIdMaxStreamFromUrlQueryHandler(IRepositoryWrapper repository)
    {
        Repository = repository;
    }

    public async Task<M3UFileIdMaxStream?> Handle(GetM3UFileIdMaxStreamFromUrlQuery request, CancellationToken cancellationToken = default)
    {

        VideoStream? videoStream = Repository.VideoStream.GetAllVideoStreams().FirstOrDefault(a => a.User_Url == request.Url);

        if (videoStream == null)
        {
            return null;
        }

        if (videoStream.M3UFileId == 0)
        {
            return new M3UFileIdMaxStream { M3UFileId = videoStream.M3UFileId, MaxStreams = 999 };
        }

        IEnumerable<M3UFile> m3uFiles = await Repository.M3UFile.GetAllM3UFilesAsync();
        M3UFile m3uFile = m3uFiles.Single(a => a.Id == videoStream.M3UFileId);

        return new M3UFileIdMaxStream { M3UFileId = videoStream.M3UFileId, MaxStreams = m3uFile.MaxStreamCount };

    }
}
