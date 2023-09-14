using StreamMasterDomain.EPG;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFiles(EPGFileParameters Parameters) : IRequest<PagedResponse<EPGFileDto>>;

internal class GetEPGFilesHandler : BaseMediatorRequestHandler, IRequestHandler<GetEPGFiles, PagedResponse<EPGFileDto>>
{
    public GetEPGFilesHandler(ILogger<GetEPGFiles> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }

    public async Task<PagedResponse<EPGFileDto>> Handle(GetEPGFiles request, CancellationToken cancellationToken = default)
    {
        PagedResponse<EPGFileDto> epgFiles = await Repository.EPGFile.GetPagedEPGFiles(request.Parameters);

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