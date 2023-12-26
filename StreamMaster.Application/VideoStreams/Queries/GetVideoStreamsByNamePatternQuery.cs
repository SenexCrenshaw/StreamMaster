using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using System.Text.RegularExpressions;

namespace StreamMaster.Application.VideoStreams.Queries;

public record GetVideoStreamsByNamePatternQuery(string pattern) : IRequest<IEnumerable<VideoStreamDto>> { }

internal class GetVideoStreamsByNamePatternQueryHandler(ILogger<GetVideoStreamsByNamePatternQuery> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetVideoStreamsByNamePatternQuery, IEnumerable<VideoStreamDto>>
{
    public async Task<IEnumerable<VideoStreamDto>> Handle(GetVideoStreamsByNamePatternQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.pattern))
        {
            return Enumerable.Empty<VideoStreamDto>();
        }

        Regex regex = new(request.pattern, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
        List<VideoStreamDto> allVideoStreams = await Repository.VideoStream.GetVideoStreams();
        IEnumerable<VideoStreamDto> dtos = allVideoStreams.Where(vs => regex.IsMatch(vs.User_Tvg_name));

        return dtos;

    }
}