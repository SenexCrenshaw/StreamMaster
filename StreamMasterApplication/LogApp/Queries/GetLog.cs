using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.LogApp.Queries;
//public record GetLogRequest(int LastId, int MaxLines) : IRequest<List<LogEntryDto>>;

public record GetLogRequest(int LastId, int MaxLines) : IRequest<List<LogEntryDto>>;

internal class GetLogFileHandler : IRequestHandler<GetLogRequest, List<LogEntryDto>>
{
    private readonly ILogDB _logContext;
    private readonly IMapper _mapper;

    public GetLogFileHandler(ILogDB logContext, IMapper mapper)
    {
        _logContext = logContext;
        _mapper = mapper;
    }

    public async Task<List<LogEntryDto>> Handle(GetLogRequest request, CancellationToken cancellationToken = default)
    {

        int max = request.MaxLines;
        if (max < 1)
        {
            max = 500;
        }
        return await _logContext.LogEntries
                .AsNoTracking()
                .Where(a => a.Id > request.LastId)
                .ProjectTo<LogEntryDto>(_mapper.ConfigurationProvider)
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
