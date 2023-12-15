namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFile(int Id) : IRequest<EPGFileDto?>;

internal class GetEPGFileHandler(ILogger<GetEPGFile> logger, IRepositoryWrapper repository, ISchedulesDirectData schedulesDirectData, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetEPGFile, EPGFileDto?>
{
    public async Task<EPGFileDto?> Handle(GetEPGFile request, CancellationToken cancellationToken = default)
    {
        EPGFile? epgFile = await Repository.EPGFile.GetEPGFileById(request.Id).ConfigureAwait(false);
        if (epgFile == null)
        {
            return null;
        }
        EPGFileDto epgFileDto = Mapper.Map<EPGFileDto>(epgFile);

        var programmes = schedulesDirectData.Programs.Where(a => a.extras.ContainsKey("epgid") && a.extras["epgid"] == epgFileDto.Id);
        var channels = schedulesDirectData.Services.Where(a => a.extras.ContainsKey("epgid") && a.extras["epgid"] == epgFileDto.Id);

        //var c = await Sender.Send(new GetProgrammesRequest(), cancellationToken).ConfigureAwait(false);
        //  var proprammes = c.Where(a => a.EPGFileId == epgFile.Id).ToList();
        //if (proprammes.Any())
        //{
        //    epgFileDto.EPGStartDate = proprammes.Min(a => a.s);
        //    epgFileDto.EPGStopDate = proprammes.Max(a => a.StopDateTime);
        //}
        epgFileDto.ProgrammeCount = programmes.Count();
        epgFileDto.ChannelCount = channels.Count();
        return epgFileDto;
    }
}
