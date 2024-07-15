namespace StreamMaster.Application.LogApp.Queries;
//public record GetLogRequest(int LastId, int MaxLines) : IRequest<List<LogEntryDto>>;

public record GetLogRequest(int LastId, int MaxLines) : IRequest<List<LogEntryDto>>;

internal class GetLogFileHandler(ILogDB logContext, IMapper mapper) : IRequestHandler<GetLogRequest, List<LogEntryDto>>
{
    public async Task<List<LogEntryDto>> Handle(GetLogRequest request, CancellationToken cancellationToken = default)
    {

        int max = request.MaxLines;
        if (max < 1)
        {
            max = 500;
        }
        return await logContext.LogEntries
                .AsNoTracking()
                .Where(a => a.Id > request.LastId)
                .ProjectTo<LogEntryDto>(mapper.ConfigurationProvider)
                .OrderByDescending(a => a.TimeStamp)
                .Take(max)
                .OrderBy(a => a.TimeStamp)
                .ToListAsync(cancellationToken: cancellationToken);
        //}

        //return _logContext.LogEntries
        //            .AsNoTracking()
        //            .Where(a => a.Id > request.LastId)
        //            .ProjectTo<LogEntryDto>(_mapper.ConfigurationProvider)
        //            .OrderBy(a => a.TimeStamp)
        //            .ToList();
    }
}
