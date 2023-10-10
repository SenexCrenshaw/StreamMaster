using FluentValidation;

using StreamMasterApplication.Programmes.Queries;

using StreamMasterDomain.EPG;

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

public class ProcessEPGFileRequestHandler(ILogger<ProcessEPGFileRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<ProcessEPGFileRequest, EPGFileDto?>
{
    [LogExecutionTimeAspect]
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

    [LogExecutionTimeAspect]
    private async Task AddProgrammesFromEPG(EPGFile epgFile, CancellationToken cancellationToken = default)
    {
        try
        {
            List<Programme> cacheValues = await Sender.Send(new GetProgrammes(), cancellationToken).ConfigureAwait(false);// MemoryCache.Programmes();
            //if (MemoryCache.ProgrammeIcons().Count == 0)
            //{
            //    DateTime start = DateTime.Now.AddDays(-1);
            //    DateTime end = DateTime.Now.AddDays(7);

            //    cacheValue.Add(new Programme
            //    {
            //        Channel = "Dummy",
            //        ChannelName = "Dummy",
            //        DisplayName = "Dummy",
            //        Start = start.AddDays(-1).ToString("yyyyMMddHHmmss") + " +0000",
            //        Stop = end.ToString("yyyyMMddHHmmss") + " +0000"
            //    });
            //}

            if (cacheValues.Count == 0)
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

                MemoryCache.SetCache(programmeChannels);
            }

            if (cancellationToken.IsCancellationRequested) { return; }

            Tv? epg = await epgFile.GetTV().ConfigureAwait(false);

            if (epg is null || epg.Programme is null)
            {
                return;
            }

            //List<EPGFileDto> epgs = await Repository.EPGFile.GetEPGFiles();
            //bool needsEpgName = epgs.Count > 1;

            // Convert the list of channels to a dictionary for faster lookups, only considering the first occurrence of each ID
            Dictionary<string?, TvChannel> channelLookup = epg.Channel
                .Where(ch => ch.Id != null)
                .GroupBy(ch => ch.Id)
                .ToDictionary(group => group.Key, group => group.First());

            foreach (Programme p in epg.Programme)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (channelLookup.TryGetValue(p.Channel, out TvChannel channel))
                {
                    string channelNameSuffix = channel.Displayname?.LastOrDefault();

                    if (channelNameSuffix != null && channelNameSuffix != p.Channel)
                    {
                        p.DisplayName = epgFile.Name + " : " + channelNameSuffix;
                        p.ChannelName = p.Channel + " - " + channelNameSuffix;
                        p.Name = channelNameSuffix;
                    }
                    else
                    {
                        p.DisplayName = epgFile.Name + " : " + p.Channel;
                        p.ChannelName = p.Channel;
                        p.Name = p.Channel;
                    }
                }
                else
                {
                    p.DisplayName = epgFile.Name + " : " + p.Channel;
                    p.ChannelName = p.Channel;
                    p.Name = p.Channel;
                }

                p.EPGFileId = epgFile.Id;
                cacheValues.Add(p);
            }

            MemoryCache.SetCache(cacheValues);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
        }
        return;
    }
}