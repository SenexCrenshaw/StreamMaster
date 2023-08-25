using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFiles(EPGFileParameters Parameters) : IRequest<PagedResponse<EPGFilesDto>>;

internal class GetEPGFilesHandler : BaseMemoryRequestHandler, IRequestHandler<GetEPGFiles, PagedResponse<EPGFilesDto>>
{
    public GetEPGFilesHandler(ILogger<GetEPGFilesHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<PagedResponse<EPGFilesDto>> Handle(GetEPGFiles request, CancellationToken cancellationToken = default)
    {
        PagedResponse<EPGFilesDto> epgFiles = await Repository.EPGFile.GetEPGFilesAsync(request.Parameters);

        if (request.Parameters.PageSize == 0)
        {
            PagedResponse<EPGFilesDto> emptyResponse = new();
            emptyResponse.TotalItemCount = epgFiles.TotalItemCount;
            return emptyResponse;
        }


        foreach (EPGFilesDto epgFileDto in epgFiles.Data)
        {
            List<Programme> proprammes = MemoryCache.Programmes().Where(a => a.EPGFileId == epgFileDto.Id).ToList();
            if (proprammes.Any())
            {
                epgFileDto.EPGStartDate = proprammes.Min(a => a.StartDateTime);
                epgFileDto.EPGStopDate = proprammes.Max(a => a.StopDateTime);
            }
        }
        return epgFiles;
    }
}