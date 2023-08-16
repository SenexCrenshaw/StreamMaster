using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using System.Text.RegularExpressions;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreamsByNamePatternQuery(string pattern) : IRequest<IQueryable<VideoStream>> { }

internal class GetVideoStreamsByNamePatternQueryHandler : BaseRequestHandler, IRequestHandler<GetVideoStreamsByNamePatternQuery, IQueryable<VideoStream>>
{
    public GetVideoStreamsByNamePatternQueryHandler(ILogger<ChangeM3UFileNameRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public Task<IQueryable<VideoStream>?> Handle(GetVideoStreamsByNamePatternQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.pattern))
        {
            return Task.FromResult<IQueryable<VideoStream>?>(null);
        }

        Regex regex = new(request.pattern, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
        IQueryable<VideoStream> allVideoStreams = Repository.VideoStream.GetAllVideoStreams();
        return Task.FromResult(allVideoStreams.Where(vs => regex.IsMatch(vs.User_Tvg_name)));
    }
}