using StreamMasterApplication.Common.Extensions;

using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFiles(EPGFileParameters Parameters) : IRequest<PagedResponse<EPGFileDto>>;

internal class GetEPGFilesHandler : BaseMemoryRequestHandler, IRequestHandler<GetEPGFiles, PagedResponse<EPGFileDto>>
{
    public GetEPGFilesHandler(ILogger<GetEPGFiles> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, publisher, sender, hubContext, memoryCache) { }

    public async Task<PagedResponse<EPGFileDto>> Handle(GetEPGFiles request, CancellationToken cancellationToken = default)
    {
        PagedResponse<EPGFileDto> epgFiles = await Repository.EPGFile.GetEPGFilesAsync(request.Parameters);

        if (request.Parameters.PageSize == 0)
        {
            return request.Parameters.CreateEmptyPagedResponse<EPGFileDto>();
        }


        foreach (EPGFileDto epgFileDto in epgFiles.Data)
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