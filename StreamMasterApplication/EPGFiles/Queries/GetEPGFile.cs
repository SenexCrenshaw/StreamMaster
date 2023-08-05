using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFile(int Id) : IRequest<EPGFilesDto?>;

internal class GetEPGFileHandler : BaseMemoryRequestHandler, IRequestHandler<GetEPGFile, EPGFilesDto?>
{


    public GetEPGFileHandler(ILogger<ProcessM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<EPGFilesDto?> Handle(GetEPGFile request, CancellationToken cancellationToken = default)
    {
        EPGFile? epgFile = await Repository.EPGFile.GetEPGFileByIdAsync(request.Id).ConfigureAwait(false);
        if (epgFile == null)
        {
            return null;
        }

        EPGFilesDto ret = Mapper.Map<EPGFilesDto>(epgFile);

        List<Programme> proprammes = MemoryCache.Programmes().Where(a => a.EPGFileId == epgFile.Id).ToList();
        if (proprammes.Any())
        {
            ret.EPGStartDate = proprammes.Min(a => a.StartDateTime);
            ret.EPGStopDate = proprammes.Max(a => a.StopDateTime);
        }

        return ret;
    }
}
