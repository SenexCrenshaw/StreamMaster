using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.M3UFiles.Commands;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreamNamesRequest() : IRequest<List<IdName>>;

internal class GetVideoStreamNamesHandler(ILogger<ChangeM3UFileNameRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService) : BaseRequestHandler(logger, repository, mapper, settingsService), IRequestHandler<GetVideoStreamNamesRequest, List<IdName>>
{
    public async Task<List<IdName>> Handle(GetVideoStreamNamesRequest request, CancellationToken cancellationToken)
    {
        List<IdName> matchedIds = await Repository.VideoStream.GetVideoStreamQuery()
            .Where(vs => !vs.IsHidden)
            .OrderBy(vs => vs.User_Tvg_name)
            .Select(vs => new IdName(vs.Id, vs.User_Tvg_name))
            .ToListAsync(cancellationToken: cancellationToken);


        return matchedIds;

    }
}