using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFile(int Id) : IRequest<EPGFilesDto?>;

internal class GetEPGFileHandler : IRequestHandler<GetEPGFile, EPGFilesDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public GetEPGFileHandler(
         IMapper mapper,
        IMemoryCache memoryCache,
        IAppDbContext context)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
        _context = context;
    }

    public async Task<EPGFilesDto?> Handle(GetEPGFile request, CancellationToken cancellationToken = default)
    {
        EPGFile? EPGFile = await _context.EPGFiles.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (EPGFile == null)
        {
            return null;
        }

        EPGFilesDto ret = _mapper.Map<EPGFilesDto>(EPGFile);

        List<StreamMasterDomain.Entities.EPG.Programme> proprammes = _memoryCache.Programmes().Where(a => a.EPGFileId == EPGFile.Id).ToList();
        if (proprammes.Any())
        {
            ret.EPGStartDate = proprammes.Min(a => a.StartDateTime);
            ret.EPGStopDate = proprammes.Max(a => a.StopDateTime);
        }

        return ret;
    }
}
