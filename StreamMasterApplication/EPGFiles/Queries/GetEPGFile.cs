using StreamMasterDomain.EPG;

namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFile(int Id) : IRequest<EPGFileDto?>;

internal class GetEPGFileHandler : BaseMediatorRequestHandler, IRequestHandler<GetEPGFile, EPGFileDto?>
{

    public GetEPGFileHandler(ILogger<GetEPGFile> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }
    public async Task<EPGFileDto?> Handle(GetEPGFile request, CancellationToken cancellationToken = default)
    {
        EPGFileDto? epgFile = await Repository.EPGFile.GetEPGFileById(request.Id).ConfigureAwait(false);
        if (epgFile == null)
        {
            return null;
        }

        List<Programme> proprammes = MemoryCache.Programmes().Where(a => a.EPGFileId == epgFile.Id).ToList();
        if (proprammes.Any())
        {
            epgFile.EPGStartDate = proprammes.Min(a => a.StartDateTime);
            epgFile.EPGStopDate = proprammes.Max(a => a.StopDateTime);
        }

        return epgFile;
    }
}
