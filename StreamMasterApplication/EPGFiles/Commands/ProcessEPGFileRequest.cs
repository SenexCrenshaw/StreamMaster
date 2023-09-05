using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Hubs;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository.EPG;

using System.ComponentModel.DataAnnotations;

namespace StreamMasterApplication.EPGFiles.Commands;

public class ProcessEPGFileRequest : IRequest<EPGFilesDto?>
{
    [Required]
    public int Id { get; set; }
}

public class ProcessEPGFileRequestValidator : AbstractValidator<ProcessEPGFileRequest>
{
    public ProcessEPGFileRequestValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

public class ProcessEPGFileRequestHandler : BaseMemoryRequestHandler, IRequestHandler<ProcessEPGFileRequest, EPGFilesDto?>
{

    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public ProcessEPGFileRequestHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, ILogger<ProcessEPGFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { _hubContext = hubContext; }

    public async Task<EPGFilesDto?> Handle(ProcessEPGFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            EPGFile? epgFile = await Repository.EPGFile.GetEPGFileByIdAsync(request.Id).ConfigureAwait(false);
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

            await Repository.SaveAsync().ConfigureAwait(false);

            await AddProgrammesFromEPG(epgFile, cancellationToken);

            EPGFilesDto ret = Mapper.Map<EPGFilesDto>(epgFile);

            await _hubContext.Clients.All.ProgrammesRefresh().ConfigureAwait(false);

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

        IEnumerable<EPGFile> epgs = await Repository.EPGFile.GetAllEPGFilesAsync();
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
