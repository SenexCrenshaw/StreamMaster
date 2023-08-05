using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFiles(EPGFileParameters Parameters) : IRequest<IEnumerable<EPGFilesDto>>;

internal class GetEPGFilesHandler : BaseMemoryRequestHandler, IRequestHandler<GetEPGFiles, IEnumerable<EPGFilesDto>>
{

    public GetEPGFilesHandler(ILogger<ProcessM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<IEnumerable<EPGFilesDto>> Handle(GetEPGFiles request, CancellationToken cancellationToken = default)
    {
        PagedList<EPGFile> epgFiles = await Repository.EPGFile.GetEPGFilesAsync(request.Parameters);

        IEnumerable<EPGFilesDto> ret = Mapper.Map<IEnumerable<EPGFilesDto>>(epgFiles);

        foreach (EPGFilesDto epgFileDto in ret)
        {
            List<Programme> proprammes = MemoryCache.Programmes().Where(a => a.EPGFileId == epgFileDto.Id).ToList();
            if (proprammes.Any())
            {
                epgFileDto.EPGStartDate = proprammes.Min(a => a.StartDateTime);
                epgFileDto.EPGStopDate = proprammes.Max(a => a.StopDateTime);
            }
        }
        return ret;
    }
}
