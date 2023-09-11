using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFile(int Id) : IRequest<EPGFileDto?>;

internal class GetEPGFileHandler : BaseMemoryRequestHandler, IRequestHandler<GetEPGFile, EPGFileDto?>
{

    public GetEPGFileHandler(ILogger<GetEPGFile> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache) { }
    public async Task<EPGFileDto?> Handle(GetEPGFile request, CancellationToken cancellationToken = default)
    {
        EPGFile? epgFile = await Repository.EPGFile.GetEPGFileByIdAsync(request.Id).ConfigureAwait(false);
        if (epgFile == null)
        {
            return null;
        }

        EPGFileDto ret = Mapper.Map<EPGFileDto>(epgFile);

        List<Programme> proprammes = MemoryCache.Programmes().Where(a => a.EPGFileId == epgFile.Id).ToList();
        if (proprammes.Any())
        {
            ret.EPGStartDate = proprammes.Min(a => a.StartDateTime);
            ret.EPGStopDate = proprammes.Max(a => a.StopDateTime);
        }

        return ret;
    }
}
