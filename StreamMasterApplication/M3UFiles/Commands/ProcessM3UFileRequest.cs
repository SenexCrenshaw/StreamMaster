using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace StreamMasterApplication.M3UFiles.Commands;

public class ProcessM3UFileRequest : IRequest<M3UFilesDto?>
{
    [Required]
    public int M3UFileId { get; set; }
}

public class ProcessM3UFileRequestValidator : AbstractValidator<ProcessM3UFileRequest>
{
    public ProcessM3UFileRequestValidator()
    {
        _ = RuleFor(v => v.M3UFileId)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

public class ProcessM3UFileRequestHandler : IRequestHandler<ProcessM3UFileRequest, M3UFilesDto?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<ProcessM3UFileRequestHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public ProcessM3UFileRequestHandler(
        ILogger<ProcessM3UFileRequestHandler> logger,

        IMapper mapper,
          IPublisher publisher,
        IAppDbContext context
    )
    {
        _publisher = publisher;
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<M3UFilesDto?> Handle(ProcessM3UFileRequest command, CancellationToken cancellationToken)
    {
        try
        {
            M3UFile? m3uFile = await _context.M3UFiles.FindAsync(new object?[] { command.M3UFileId }, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (m3uFile == null)
            {
                _logger.LogCritical("Could not find M3U file");
                return null;
            }

            List<VideoStream>? streams = await m3uFile.GetM3U().ConfigureAwait(false);
            if (streams == null)
            {
                _logger.LogCritical("Error while processing M3U file, bad format");
                return null;
            }
            Setting setting = FileUtil.GetSetting();

            Stopwatch sw = Stopwatch.StartNew();
            var existing = _context.VideoStreams.Where(a => a.M3UFileId == m3uFile.Id);

            var existingChannels = new ThreadSafeIntList(m3uFile.StartingChannelNumber < 1 ? 1 : m3uFile.StartingChannelNumber);

            //var newChannels = streams.Select(a => a.Tvg_chno).Distinct().Order().ToList();

            var groups = _context.ChannelGroups.ToList();
            int nextchno = setting.FirstFreeNumber;

            for (int index = 0; index < streams.Count; index++)
            {
                VideoStream? stream = streams[index];

                var group = groups.FirstOrDefault(a => a.Name.ToLower() == stream.Tvg_group.ToLower());

                if (existing.Any() && existing.Any(a => a.Id == stream.Id))
                {
                    try
                    {
                        VideoStream dbStream = existing.Single(a => a.Id == stream.Id);

                        if (group != null)
                        {
                            stream.IsHidden = dbStream.IsHidden ? dbStream.IsHidden : group.IsHidden;
                        }

                        if (dbStream.Tvg_group == dbStream.User_Tvg_group)
                        {
                            dbStream.User_Tvg_group = stream.Tvg_group;
                        }
                        dbStream.Tvg_group = stream.Tvg_group;

                        if (stream.Tvg_chno == 0 || setting.OverWriteM3UChannels || existingChannels.ContainsInt(stream.Tvg_chno))
                        {
                            nextchno = existingChannels.GetNextInt(nextchno);

                            dbStream.User_Tvg_chno = nextchno;
                            dbStream.Tvg_chno = nextchno;
                        }
                        else
                        {
                            if (dbStream.User_Tvg_chno == 0 || dbStream.Tvg_chno == dbStream.User_Tvg_chno)
                            {
                                dbStream.User_Tvg_chno = stream.Tvg_chno;
                            }
                            dbStream.Tvg_chno = stream.Tvg_chno;
                        }

                        if (dbStream.Tvg_ID == dbStream.User_Tvg_ID)
                        {
                            dbStream.User_Tvg_ID = stream.Tvg_ID;
                        }
                        dbStream.Tvg_ID = stream.Tvg_ID;

                        if (dbStream.Tvg_logo == dbStream.User_Tvg_logo)
                        {
                            dbStream.User_Tvg_logo = stream.Tvg_logo;
                        }
                        dbStream.Tvg_logo = stream.Tvg_logo;

                        if (dbStream.Tvg_name == dbStream.User_Tvg_name)
                        {
                            dbStream.User_Tvg_name = stream.Tvg_name;
                        }
                        dbStream.Tvg_name = stream.Tvg_name;

                        dbStream.FilePosition = index;
                        //dbStream.Url = stream.Url;
                        //dbStream.User_Url = stream.Url;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error while processing M3U file, duplicate Id for {stream.Id}", ex);
                    }
                }
                else
                {
                    stream.IsHidden = group != null && group.IsHidden;
                    if (stream.User_Tvg_chno == 0 || setting.OverWriteM3UChannels || existingChannels.ContainsInt(stream.Tvg_chno))
                    {
                        nextchno = existingChannels.GetNextInt(nextchno);
                        stream.User_Tvg_chno = nextchno;
                        stream.Tvg_chno = nextchno;
                    }
                    stream.FilePosition = index;
                    _ = _context.VideoStreams.Add(stream);
                }
            }

            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            sw.Stop();
            Console.Write($"Update of{m3uFile.Id} {m3uFile.Name}, took {sw.Elapsed.TotalSeconds} seconds");

            if (m3uFile.StationCount != streams.Count)
            {
                m3uFile.StationCount = streams.Count;
            }

            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            List<string> newGroups = streams.Where(a => a.User_Tvg_group != null).Select(a => a.User_Tvg_group).Distinct().ToList();

            int rank = _context.ChannelGroups.Any() ? _context.ChannelGroups.Max(a => a.Rank) + 1 : 1;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            foreach (string? ng in newGroups)
            {
                if (!_context.ChannelGroups.Any(a => a.Name.ToLower() == ng.ToLower()))
                {
                    ChannelGroup channelGroup = new()
                    {
                        Name = ng,
                        Rank = rank++,
                        IsReadOnly = true,
                    };

                    _ = _context.ChannelGroups.Add(channelGroup);
                }
            }
            if (await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0)
            {
                await _publisher.Publish(new AddChannelGroupsEvent(), cancellationToken).ConfigureAwait(false);
            }

            M3UFilesDto ret = _mapper.Map<M3UFilesDto>(m3uFile);
            await _publisher.Publish(new M3UFileProcessedEvent(ret), cancellationToken).ConfigureAwait(false);

            return ret;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error while processing M3U file");
        }

        return null;
    }
}
