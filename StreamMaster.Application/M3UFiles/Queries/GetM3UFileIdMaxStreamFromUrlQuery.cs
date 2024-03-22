using StreamMaster.Domain.Common;
using StreamMaster.Domain.Models;
using StreamMaster.Domain.Repository;

namespace StreamMaster.Application.M3UFiles.Queries;

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

        VideoStream? videoStream = Repository.VideoStream.GetVideoStreamQuery().FirstOrDefault(a => a.User_Url == request.Url);

        if (videoStream == null)
        {
            return null;
        }

        if (videoStream.M3UFileId == 0)
        {
            return new M3UFileIdMaxStream { M3UFileId = videoStream.M3UFileId, MaxStreams = 999 };
        }

        M3UFile? m3uFile = await Repository.M3UFile.GetM3UFile(videoStream.M3UFileId);

        return new M3UFileIdMaxStream { M3UFileId = videoStream.M3UFileId, MaxStreams = m3uFile?.MaxStreamCount ?? 999 };

    }
}
