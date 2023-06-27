using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.Hubs;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities.EPG;

using System.ComponentModel.DataAnnotations;

namespace StreamMasterApplication.EPGFiles.Commands;

public class ProcessEPGFileRequest : IRequest<EPGFilesDto?>
{
    [Required]
    public int EPGFileId { get; set; }
}

public class ProcessEPGFileRequestValidator : AbstractValidator<ProcessEPGFileRequest>
{
    public ProcessEPGFileRequestValidator()
    {
        _ = RuleFor(v => v.EPGFileId)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

public class ProcessEPGFileRequestHandler : IRequestHandler<ProcessEPGFileRequest, EPGFilesDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly IPublisher _publisher;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public ProcessEPGFileRequestHandler(
           IPublisher publisher,
        IAppDbContext context,
        IMapper mapper,
      IHubContext<StreamMasterHub, IStreamMasterHub> hubContext,
        IMemoryCache memoryCache
    )
    {
        _hubContext = hubContext;
        _publisher = publisher;
        _context = context;
        _mapper = mapper;
        _memoryCache = memoryCache;
    }

    public async Task<EPGFilesDto?> Handle(ProcessEPGFileRequest command, CancellationToken cancellationToken)
    {
        try
        {
            EPGFile? epgFile = await _context.EPGFiles.FindAsync(new object?[] { command.EPGFileId }, cancellationToken: cancellationToken).ConfigureAwait(false);
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
            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await AddProgrammesFromEPG(epgFile, cancellationToken);

            EPGFilesDto ret = _mapper.Map<EPGFilesDto>(epgFile);
            
             await _hubContext.Clients.All.ProgrammeNamesUpdate(_memoryCache.Programmes()).ConfigureAwait(false);

            await _publisher.Publish(new EPGFileProcessedEvent(ret), cancellationToken).ConfigureAwait(false);

            return ret;
        }
        catch (Exception)
        {
        }
        return null;
    }

    private async Task AddProgrammesFromEPG(EPGFile epgFile, CancellationToken cancellationToken = default)
    {
        if (!_memoryCache.TryGetValue(CacheKeys.ListProgrammes, out List<Programme>? cacheValue))
        {
            cacheValue = new List<Programme>();

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

        if (!_memoryCache.TryGetValue(CacheKeys.ListProgrammeChannel, out List<ProgrammeChannel>? programmeChannels))
        {
            DateTime start = DateTime.Now.AddDays(-1);
            DateTime end = DateTime.Now.AddDays(7);

            programmeChannels = new List<ProgrammeChannel>(){
                new ProgrammeChannel
                {
                    Channel = "Dummy",
                    StartDateTime = start,
                    EndDateTime = end,
                    ProgrammeCount = 1
                }
            };

            _memoryCache.Set(programmeChannels);
        }

        if (cancellationToken.IsCancellationRequested) { return; }

        Tv? epg = await epgFile.GetTV().ConfigureAwait(false);

        if (epg is null || epg.Programme is null)
        {
            return;
        }

        var needsEpgName = _context.EPGFiles.Count() > 1;

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
        _memoryCache.Set(cacheValue);
        
        return;
    }
}
