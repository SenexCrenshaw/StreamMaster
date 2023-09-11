namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFilesNeedUpdating() : IRequest<IEnumerable<EPGFileDto>>;

internal class GetEPGFilesNeedUpdatingHandler : BaseMemoryRequestHandler, IRequestHandler<GetEPGFilesNeedUpdating, IEnumerable<EPGFileDto>>
{

    public GetEPGFilesNeedUpdatingHandler(ILogger<GetEPGFilesNeedUpdating> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
 : base(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache) { }

    public async Task<IEnumerable<EPGFileDto>> Handle(GetEPGFilesNeedUpdating request, CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.Now;

        List<EPGFile> epgFiles = new();
        IEnumerable<EPGFile> epgFilesRepo = await Repository.EPGFile.GetAllEPGFilesAsync();
        IEnumerable<EPGFile> epgFilesToUpdated = epgFilesRepo.Where(a => a.AutoUpdate && !string.IsNullOrEmpty(a.Url) && a.HoursToUpdate > 0 && a.LastDownloaded.AddHours(a.HoursToUpdate) < now);
        epgFiles.AddRange(epgFilesToUpdated);
        foreach (EPGFile? epgFile in epgFilesRepo.Where(a => string.IsNullOrEmpty(a.Url)))
        {
            if (epgFile.LastWrite() >= epgFile.LastUpdated)
            {
                epgFiles.Add(epgFile);
            }
        }
        IEnumerable<EPGFileDto> ret = Mapper.Map<IEnumerable<EPGFileDto>>(epgFiles);
        return ret;
    }
}