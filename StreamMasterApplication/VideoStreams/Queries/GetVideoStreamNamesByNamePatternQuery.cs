using System.Text.RegularExpressions;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreamNamesByNamePatternQuery(string pattern) : IRequest<IEnumerable<string>> { }

internal class GetVideoStreamNamesByNamePatternQueryHandler(ILogger<GetVideoStreamNamesByNamePatternQuery> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetVideoStreamNamesByNamePatternQuery, IEnumerable<string>>
{
    public async Task<IEnumerable<string>> Handle(GetVideoStreamNamesByNamePatternQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.pattern))
        {
            return Enumerable.Empty<string>();
        }

        Regex regex = new(request.pattern, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
        List<string> allVideoStreamNames = await Repository.VideoStream.GetVideoStreamNames().ConfigureAwait(false);
        IEnumerable<string> filtered = allVideoStreamNames.Where(vs => regex.IsMatch(vs)).Order();

        return filtered;

    }
}