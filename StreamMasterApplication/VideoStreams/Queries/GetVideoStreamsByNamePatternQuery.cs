using Microsoft.EntityFrameworkCore;

using System.Text.RegularExpressions;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreamsByNamePatternQuery(string pattern) : IRequest<IEnumerable<VideoStreamDto>> { }

internal class GetVideoStreamsByNamePatternQueryHandler(ILogger<GetVideoStreamsByNamePatternQuery> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper,settingsService, publisher, sender, hubContext), IRequestHandler<GetVideoStreamsByNamePatternQuery, IEnumerable<VideoStreamDto>>
{
    public async Task<IEnumerable<VideoStreamDto>> Handle(GetVideoStreamsByNamePatternQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.pattern))
        {
            return Enumerable.Empty<VideoStreamDto>();
        }

        Regex regex = new(request.pattern, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
        List<VideoStream> allVideoStreams = await Repository.VideoStream.GetJustVideoStreams().ToListAsync();
        IEnumerable<VideoStream> filtered = allVideoStreams.Where(vs => regex.IsMatch(vs.User_Tvg_name));
        IEnumerable<VideoStreamDto> dtos = Mapper.Map<IEnumerable<VideoStreamDto>>(filtered);

        return dtos;

    }
}