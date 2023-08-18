using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;

using System.Diagnostics;
using System.Text.RegularExpressions;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreamsByNamePatternQuery(string pattern) : IRequest<IEnumerable<VideoStreamDto>> { }

internal class GetVideoStreamsByNamePatternQueryHandler : BaseMediatorRequestHandler, IRequestHandler<GetVideoStreamsByNamePatternQuery, IEnumerable<VideoStreamDto>>
{
    public GetVideoStreamsByNamePatternQueryHandler(ILogger<GetVideoStreamsByNamePatternQuery> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<IEnumerable<VideoStreamDto>> Handle(GetVideoStreamsByNamePatternQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.pattern))
        {
            return Enumerable.Empty<VideoStreamDto>();
        }
        Stopwatch stopwatch = Stopwatch.StartNew();

        Regex regex = new(request.pattern, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
        List<VideoStream> allVideoStreams = await Repository.VideoStream.GetJustVideoStreams().ToListAsync();
        IEnumerable<VideoStream> filtered = allVideoStreams.Where(vs => regex.IsMatch(vs.User_Tvg_name));
        IEnumerable<VideoStreamDto> dtos = Mapper.Map<IEnumerable<VideoStreamDto>>(filtered);
        stopwatch.Stop();
        Logger.LogInformation($"ElapsedMilliseconds: {stopwatch.ElapsedMilliseconds} ms , got {dtos.Count()} items for pattern {request.pattern}");

        return dtos;

    }
}