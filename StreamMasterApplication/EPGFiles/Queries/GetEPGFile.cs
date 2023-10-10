using StreamMasterApplication.Programmes.Queries;

using StreamMasterDomain.EPG;

namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFile(int Id) : IRequest<EPGFileDto?>;

internal class GetEPGFileHandler : BaseMediatorRequestHandler, IRequestHandler<GetEPGFile, EPGFileDto?>
{

    public GetEPGFileHandler(ILogger<GetEPGFile> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }
    public async Task<EPGFileDto?> Handle(GetEPGFile request, CancellationToken cancellationToken = default)
    {
        EPGFile? epgFile = await Repository.EPGFile.GetEPGFileById(request.Id).ConfigureAwait(false);
        if (epgFile == null)
        {
            return null;
        }
        EPGFileDto epgFileDto = Mapper.Map<EPGFileDto>(epgFile);

        List<Programme> c = await Sender.Send(new GetProgrammes(), cancellationToken).ConfigureAwait(false);
        List<Programme> proprammes = c.Where(a => a.EPGFileId == epgFile.Id).ToList();
        if (proprammes.Any())
        {
            epgFileDto.EPGStartDate = proprammes.Min(a => a.StartDateTime);
            epgFileDto.EPGStopDate = proprammes.Max(a => a.StopDateTime);
        }

        return epgFileDto;
    }
}
