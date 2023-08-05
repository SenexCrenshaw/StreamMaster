using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using System.Text.RegularExpressions;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreamsByNamePatternQuery(string pattern) : IRequest<IEnumerable<VideoStream>> { }

internal class GetVideoStreamsByNamePatternQueryHandler : BaseRequestHandler, IRequestHandler<GetVideoStreamsByNamePatternQuery, IEnumerable<VideoStream>>
{

    public GetVideoStreamsByNamePatternQueryHandler(ILogger<ChangeM3UFileNameRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }


    public async Task<IEnumerable<VideoStream>> Handle(GetVideoStreamsByNamePatternQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.pattern))
        {
            return new List<VideoStream>();
        }

        Regex regex = new(request.pattern, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
        IQueryable<VideoStream> allVideoStreams = Repository.VideoStream.GetAllVideoStreams();

        return allVideoStreams
            .Where(vs => regex.IsMatch(vs.User_Tvg_name));
    }
}