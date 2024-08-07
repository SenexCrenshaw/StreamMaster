namespace StreamMaster.Application.M3UFiles.QueriesOld;

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

        SMStream? smStream = Repository.SMStream.FirstOrDefault(a => a.Url == request.Url);

        if (smStream == null)
        {
            return null;
        }

        if (smStream.M3UFileId == 0)
        {
            return new M3UFileIdMaxStream { M3UFileId = smStream.M3UFileId, MaxStreams = 999 };
        }

        M3UFile? m3uFile = await Repository.M3UFile.GetM3UFileAsync(smStream.M3UFileId);

        return new M3UFileIdMaxStream { M3UFileId = smStream.M3UFileId, MaxStreams = m3uFile?.MaxStreamCount ?? 999 };

    }
}
