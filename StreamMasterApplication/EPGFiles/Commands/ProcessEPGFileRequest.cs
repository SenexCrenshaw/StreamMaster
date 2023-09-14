using FluentValidation;

using StreamMasterDomain.EPG;
using StreamMasterDomain.Models;

namespace StreamMasterApplication.EPGFiles.Commands;

public record ProcessEPGFileRequest(int Id) : IRequest<EPGFileDto?> { }

public class ProcessEPGFileRequestValidator : AbstractValidator<ProcessEPGFileRequest>
{
    public ProcessEPGFileRequestValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}
public class ProcessEPGFileRequestHandler : BaseMediatorRequestHandler, IRequestHandler<ProcessEPGFileRequest, EPGFileDto?>
{

    public ProcessEPGFileRequestHandler(ILogger<ProcessEPGFileRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }

    public async Task<EPGFileDto?> Handle(ProcessEPGFileRequest request, CancellationToken cancellationToken)
    {
        try
        {

            EPGFile? epgFile = await Repository.EPGFile.GetEPGFileById(request.Id).ConfigureAwait(false);

            if (epgFile == null)
            {
                return null;
            }

            Tv? tv = await epgFile.GetTV().ConfigureAwait(false);
            if (tv != null)
            {
                epgFile.ChannelCount = tv.Channel != null ? tv.Channel.Count : 0;
                epgFile.ProgrammeCount = tv.Programme != null ? tv.Programme.Count : 0;

            }

            epgFile.LastUpdated = DateTime.Now;
            Repository.EPGFile.UpdateEPGFile(epgFile);

            _ = await Repository.SaveAsync().ConfigureAwait(false);

            await AddProgrammesFromEPG(epgFile, cancellationToken);

            EPGFileDto ret = Mapper.Map<EPGFileDto>(epgFile);

            await HubContext.Clients.All.ProgrammesRefresh().ConfigureAwait(false);

            await Publisher.Publish(new EPGFileProcessedEvent(ret), cancellationToken).ConfigureAwait(false);

            return ret;
        }
        catch (Exception)
        {
        }
        return null;
    }

    private async Task AddProgrammesFromEPG(EPGFile epgFile, CancellationToken cancellationToken = default)
    {
        List<Programme> cacheValue = new();
        if (MemoryCache.ProgrammeIcons().Count == 0)
        {
            DateTime start = DateTime.Now.AddDays(-1);
            DateTime end = DateTime.Now.AddDays(7);

            cacheValue.Add(new Programme
            {
                Channel = "Dummy",
                ChannelName = "Dummy",
                DisplayName = "Dummy",
                Start = start.AddDays(-1).ToString("yyyyMMddHHmmss") + " +0000",
                Stop = end.ToString("yyyyMMddHHmmss") + " +0000"
            });
        }

        if (MemoryCache.ProgrammeChannels().Count == 0)
        {
            DateTime start = DateTime.Now.AddDays(-1);
            DateTime end = DateTime.Now.AddDays(7);

            List<ProgrammeChannel> programmeChannels = new(){
                new ProgrammeChannel
                {
                    Channel = "Dummy",
                    StartDateTime = start,
                    EndDateTime = end,
                    ProgrammeCount = 1
                }
            };

            MemoryCache.Set(programmeChannels);
        }

        if (cancellationToken.IsCancellationRequested) { return; }

        Tv? epg = await epgFile.GetTV().ConfigureAwait(false);

        if (epg is null || epg.Programme is null)
        {
            return;
        }

        List<EPGFileDto> epgs = await Repository.EPGFile.GetEPGFiles();
        bool needsEpgName = epgs.Count() > 1;

        foreach (Programme p in epg.Programme)
        {
            if (cancellationToken.IsCancellationRequested) { break; }
            string channel_name = p.Channel;
            p.DisplayName = p.Channel;

            TvChannel? channel = epg.Channel.FirstOrDefault(a => a.Id is not null && a.Id.ToLower() == p.Channel.ToLower());

            if (channel != null && channel.Displayname is not null && channel.Displayname.Any())
            {
                if (channel.Displayname.Last() != channel_name)
                {
                    channel_name += " - " + channel.Displayname.Last();
                    p.DisplayName = channel.Displayname.Last();
                }
            }
            if (needsEpgName)
            {
                p.DisplayName = epgFile.Name + " : " + p.DisplayName;
            }

            p.ChannelName = channel_name;
            p.EPGFileId = epgFile.Id;
            p.Channel = p.Channel;
            cacheValue.Add(p);
        }
        MemoryCache.Set(cacheValue);

        return;
    }
}
