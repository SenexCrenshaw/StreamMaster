using System.Text.RegularExpressions;

namespace StreamMaster.Application.VideoStreams.Queries;

public record GetVideoStreamsByNamePatternQuery(string pattern) : IRequest<IEnumerable<VideoStreamDto>> { }

internal class GetVideoStreamsByNamePatternQueryHandler(ILogger<GetVideoStreamsByNamePatternQuery> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetVideoStreamsByNamePatternQuery, IEnumerable<VideoStreamDto>>
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