using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFiles : IRequest<IEnumerable<EPGFilesDto>>;

internal class GetEPGFilesHandler : IRequestHandler<GetEPGFiles, IEnumerable<EPGFilesDto>>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public GetEPGFilesHandler(
            IMapper mapper,
             IMemoryCache memoryCache,
            IAppDbContext context
        )
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
        _context = context;
    }

    public async Task<IEnumerable<EPGFilesDto>> Handle(GetEPGFiles request, CancellationToken cancellationToken = default)
    {
        List<EPGFilesDto> ret = await _context.EPGFiles
           .AsNoTracking()
           .ProjectTo<EPGFilesDto>(_mapper.ConfigurationProvider)
           .OrderBy(x => x.Name)
        .ToListAsync(cancellationToken).ConfigureAwait(false);

        foreach (EPGFilesDto epgFileDto in ret)
        {
            List<Programme> proprammes = _memoryCache.Programmes().Where(a => a.EPGFileId == epgFileDto.Id).ToList();
            if (proprammes.Any())
            {
                epgFileDto.EPGStartDate = proprammes.Min(a => a.StartDateTime);
                epgFileDto.EPGStopDate = proprammes.Max(a => a.StopDateTime);
            }
        }
        return ret;
    }
}
