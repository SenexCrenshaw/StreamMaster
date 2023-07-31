using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Extensions;

using System.Collections.Concurrent;
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

    private ThreadSafeIntList existingChannels;

    private int nextchno;

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
            Stopwatch sw = Stopwatch.StartNew();

            if (m3uFile.LastWrite() >=m3uFile.LastUpdated)
            {

                List<VideoStream>? streams = await m3uFile.GetM3U().ConfigureAwait(false);
                if (streams == null)
                {
                    _logger.LogCritical("Error while processing M3U file, bad format");
                    return null;
                }

                Setting setting = FileUtil.GetSetting();
               
                if (setting.NameRegex.Any())
                {
                    foreach (var regex in setting.NameRegex)
                    {
                        var toIgnore = ListHelper.GetMatchingProperty(streams, "Tvg_name", regex);

                        HashSet<VideoStream> matchingObjects = new HashSet<VideoStream>(toIgnore);
                        streams.RemoveAll(x => toIgnore.Contains(x));
                    }
                }
                sw.Stop();
                _logger.LogInformation($"Regex of ID: {m3uFile.Id} {m3uFile.Name}, took {sw.Elapsed.TotalSeconds} seconds");

                sw = Stopwatch.StartNew();
                var existing = _context.VideoStreams.Where(a => a.M3UFileId == m3uFile.Id).ToList();

                existingChannels = new ThreadSafeIntList(m3uFile.StartingChannelNumber < 1 ? 1 : m3uFile.StartingChannelNumber);

                var groups = _context.ChannelGroups.AsNoTracking().ToList();
                nextchno = m3uFile.StartingChannelNumber < 0 ? 0 : m3uFile.StartingChannelNumber;

                // var dupes = streams.Where(a => streams.Count(b => b.Id == a.Id) > 1).OrderBy(a => a.Id).ToList();
                var groupedStreams = streams.GroupBy(x => x.Id).ToList();

                var dupes = groupedStreams
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g.Skip(1)) // We skip the first one as it will be kept
                    .OrderBy(a => a.Id)
                    .ToList();


                if (dupes.Any())
                {
                    var fileName = $"dupes_{m3uFile.Id}.csv";

                    _logger.LogError($"Streams in M3U file ID: {m3uFile.Id} {m3uFile.Name}, have duplicate Ids, will only use the first entry, check {fileName} for more information", fileName);

                    // Remove duplicates based on id
                    streams = groupedStreams.Select(g => g.First()).ToList();

                    var lines = new List<string>
                {
                    VideoStream.GetCsvHeader()
                };

                    lines.AddRange(dupes.Select(a => a.ToString()));

                    using (StreamWriter file = new StreamWriter(fileName))
                    {
                        foreach (string line in lines)
                        {
                            file.WriteLine(line);
                        }
                    }
                }
                sw.Stop();
                _logger.LogInformation($"Dupe check ID: {m3uFile.Id} {m3uFile.Name}, took {sw.Elapsed.TotalSeconds} seconds");

                sw.Restart();

                ConcurrentBag<VideoStream> toWrite = new();
                // For progress reporting
                int totalCount = streams.Count();
                int processedCount = 0;
                int progressRecords = 5000;
              
                Stopwatch processSw = new Stopwatch();

                Parallel.ForEach(streams.Select((value, index) => new { value, index }), (stream) =>
                {
                    processSw.Start();
                    var group = groups.FirstOrDefault(a => a.Name.ToLower().Equals(stream.value.Tvg_group.ToLower()));
                    VideoStream dbStream = existing.FirstOrDefault(a => a.Id == stream.value.Id);

                    if (dbStream != null)
                    {
                        try
                        {
                            ProcessExistingStream(stream.value, dbStream, group, setting, stream.index);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error while processing M3U file ID: {m3uFile.Id} {m3uFile.Name}, duplicate Id for {stream.value.Id}", ex);
                        }
                    }
                    else
                    {
                        ProcessNewStream(stream.value, group, setting, stream.index);
                        toWrite.Add(stream.value);
                    }
                    // Report progress for every 1000 lines
                    var currentProgress = Interlocked.Increment(ref processedCount);
                    if (currentProgress % progressRecords == 0)
                    {
                        processSw.Stop();



                        // Log every 1000 items

                        var percentage = ((double)currentProgress / totalCount * 100).ToString("F2");
                        var speed = processSw.ElapsedMilliseconds.ToString("F3");
                        var avgTimePerItem = processSw.ElapsedMilliseconds / progressRecords;
                        int remainingItems = totalCount - currentProgress;
                        // Estimate remaining time
                        double estRemainingTime = (avgTimePerItem * remainingItems) / 1000;

                        _logger.LogInformation($"Progress: {percentage}%, {currentProgress}/{totalCount}, Speed: {speed} ms, ETA: {estRemainingTime} sec");
                        processSw.Restart();
                    }
                });

                _context.VideoStreams.AddRange(toWrite);
                m3uFile.LastUpdated = DateTime.Now;
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);


                if (m3uFile.StationCount != streams.Count)
                {
                    m3uFile.StationCount = streams.Count;
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }

                await AddChannelGroupsFromStreams(streams, cancellationToken).ConfigureAwait(false);
            }

            sw.Stop();
            _logger.LogInformation($"Update of ID: {m3uFile.Id} {m3uFile.Name}, took {sw.Elapsed.TotalSeconds} seconds");


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

    private async Task AddChannelGroupsFromStreams(IEnumerable<VideoStream> streams, CancellationToken cancellationToken)
    {
        List<string> newGroups = streams.Where(a => a.User_Tvg_group != null).Select(a => a.User_Tvg_group).Distinct().ToList();
        var channelGroups = _context.ChannelGroups.ToList();
        int rank = _context.ChannelGroups.Any() ? _context.ChannelGroups.Max(a => a.Rank) + 1 : 1;

        foreach (string? ng in newGroups)
        {
            if (!channelGroups.Any(a => string.Equals(a.Name, ng, StringComparison.OrdinalIgnoreCase)))
            {
                ChannelGroup channelGroup = new()
                {
                    Name = ng,
                    Rank = rank++,
                    IsReadOnly = true,
                };

                await _context.ChannelGroups.AddAsync(channelGroup, cancellationToken);
            }
        }

        if (await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0)
        {
            await _publisher.Publish(new AddChannelGroupsEvent(), cancellationToken).ConfigureAwait(false);
        }
    }

    private void ProcessExistingStream(VideoStream stream, VideoStream dbStream, ChannelGroup? group, Setting setting, int index)
    {
        if (group != null)
        {
            dbStream.IsHidden = dbStream.IsHidden || group.IsHidden;
        }

        dbStream.User_Tvg_group = dbStream.Tvg_group == dbStream.User_Tvg_group ? stream.Tvg_group : dbStream.User_Tvg_group;
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

        dbStream.User_Tvg_ID = dbStream.Tvg_ID == dbStream.User_Tvg_ID ? stream.Tvg_ID : dbStream.User_Tvg_ID;
        dbStream.Tvg_ID = stream.Tvg_ID;

        dbStream.User_Tvg_logo = dbStream.Tvg_logo == dbStream.User_Tvg_logo ? stream.Tvg_logo : dbStream.User_Tvg_logo;
        dbStream.Tvg_logo = stream.Tvg_logo;

        dbStream.User_Tvg_name = dbStream.Tvg_name == dbStream.User_Tvg_name ? stream.Tvg_name : dbStream.User_Tvg_name;
        dbStream.Tvg_name = stream.Tvg_name;

        dbStream.FilePosition = index;
    }

    private void ProcessNewStream(VideoStream stream, ChannelGroup? group, Setting setting, int index)
    {
        stream.IsHidden = group?.IsHidden ?? false;
        if (stream.User_Tvg_chno == 0 || setting.OverWriteM3UChannels || existingChannels.ContainsInt(stream.Tvg_chno))
        {
            nextchno = existingChannels.GetNextInt(nextchno);
            stream.User_Tvg_chno = nextchno;
            stream.Tvg_chno = nextchno;
        }

        stream.FilePosition = index;
    }
}