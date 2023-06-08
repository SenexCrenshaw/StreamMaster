using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.Services;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities;
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

    public ProcessEPGFileRequestHandler(
           IPublisher publisher,
        IAppDbContext context,
        IMapper mapper,
        IMemoryCache memoryCache
    )
    {
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

        Tv? epg = await epgFile.GetTV().ConfigureAwait(false);

        if (epg is null || epg.Programme is null)
        {
            return;
        }

        foreach (Programme p in epg.Programme)
        {
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

            p.ChannelName = channel_name;
            p.EPGFileId = epgFile.Id;
            p.Channel = p.Channel;
            cacheValue.Add(p);
        }
        _memoryCache.Set(cacheValue);

        return;
    }
}
