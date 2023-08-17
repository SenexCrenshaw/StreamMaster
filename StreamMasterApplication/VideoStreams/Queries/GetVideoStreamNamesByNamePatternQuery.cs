using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Text.RegularExpressions;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreamNamesByNamePatternQuery(string pattern) : IRequest<IEnumerable<string>> { }

internal class GetVideoStreamNamesByNamePatternQueryHandler : BaseMediatorRequestHandler, IRequestHandler<GetVideoStreamNamesByNamePatternQuery, IEnumerable<string>>
{
    public GetVideoStreamNamesByNamePatternQueryHandler(ILogger<GetVideoStreamNamesByNamePatternQueryHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<IEnumerable<string>> Handle(GetVideoStreamNamesByNamePatternQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.pattern))
        {
            return Enumerable.Empty<string>();
        }
        Stopwatch stopwatch = Stopwatch.StartNew();

        Regex regex = new(request.pattern, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
        List<string> allVideoStreamNames = await Repository.VideoStream.GetVideoStreamNames().ToListAsync(cancellationToken: cancellationToken);
        IEnumerable<string> filtered = allVideoStreamNames.Where(vs => regex.IsMatch(vs)).Order();
        stopwatch.Stop();
        Logger.LogInformation($"GetVideoStreamNamesByNamePatternQuery took {stopwatch.ElapsedMilliseconds} ms for pattern {request.pattern}");

        return filtered;

    }
}