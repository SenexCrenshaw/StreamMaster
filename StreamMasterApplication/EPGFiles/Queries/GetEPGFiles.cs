using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFiles(EPGFileParameters Parameters) : IRequest<StaticPagedList<EPGFilesDto>>;

internal class GetEPGFilesHandler : BaseMemoryRequestHandler, IRequestHandler<GetEPGFiles, StaticPagedList<EPGFilesDto>>
{
    public GetEPGFilesHandler(ILogger<ProcessM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<StaticPagedList<EPGFilesDto>> Handle(GetEPGFiles request, CancellationToken cancellationToken = default)
    {
        var epgFiles = await Repository.EPGFile.GetEPGFilesAsync(request.Parameters);

        IEnumerable<EPGFilesDto> epgFileDtos = Mapper.Map<IEnumerable<EPGFilesDto>>(epgFiles);

        foreach (EPGFilesDto epgFileDto in epgFileDtos)
        {
            List<Programme> proprammes = MemoryCache.Programmes().Where(a => a.EPGFileId == epgFileDto.Id).ToList();
            if (proprammes.Any())
            {
                epgFileDto.EPGStartDate = proprammes.Min(a => a.StartDateTime);
                epgFileDto.EPGStopDate = proprammes.Max(a => a.StopDateTime);
            }
        }

        var result = new StaticPagedList<EPGFilesDto>(epgFileDtos, epgFiles.GetMetaData());

        //PagedList<EPGFilesDto> result = new(epgFileDtos.ToList(), epgFiles.TotalCount, epgFiles.CurrentPage, epgFiles.PageSize);
        return result;
    }
}