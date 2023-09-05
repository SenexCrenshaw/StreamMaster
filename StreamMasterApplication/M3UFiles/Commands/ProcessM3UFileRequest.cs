using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.ChannelGroups.Commands;

using StreamMasterDomain.Extensions;

using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace StreamMasterApplication.M3UFiles.Commands;

public class ProcessM3UFileRequest : IRequest<M3UFile?>
{
    [Required]
    public int Id { get; set; }
}

public class ProcessM3UFileRequestValidator : AbstractValidator<ProcessM3UFileRequest>
{
    public ProcessM3UFileRequestValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

public class ProcessM3UFileRequestHandler : BaseMemoryRequestHandler, IRequestHandler<ProcessM3UFileRequest, M3UFile?>
{
    private ThreadSafeIntList existingChannels;

    private int nextchno;

    public ProcessM3UFileRequestHandler(ILogger<ProcessM3UFileRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<M3UFile?> Handle(ProcessM3UFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            M3UFile? m3uFile = await FetchM3UFile(request.Id).ConfigureAwait(false);
            if (m3uFile == null)
            {
                Logger.LogCritical("Could not find M3U file");
                return null;
            }

            List<VideoStream>? streams = await ProcessStreams(m3uFile).ConfigureAwait(false);
            if (streams == null)
            {
                Logger.LogCritical("Error while processing M3U file, bad format");
                return null;
            }

            if (!ShouldUpdate(m3uFile))
            {
                return m3uFile;
            }

            await ProcessAndUpdateStreams(m3uFile, streams, cancellationToken).ConfigureAwait(false);
            await UpdateChannelGroups(streams, cancellationToken).ConfigureAwait(false);
            await NotifyUpdates(m3uFile, cancellationToken).ConfigureAwait(false);

            return m3uFile;
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Error while processing M3U file");
            return null;
        }
    }

    private async Task<M3UFile?> FetchM3UFile(int id)
    {
        return await Repository.M3UFile.GetM3UFileByIdAsync(id).ConfigureAwait(false);
    }

    private async Task<List<VideoStream>?> ProcessStreams(M3UFile m3uFile)
    {
        Stopwatch sw = Stopwatch.StartNew();

        List<VideoStream>? streams = await m3uFile.GetM3U().ConfigureAwait(false);
        if (streams != null && m3uFile.LastWrite() >= m3uFile.LastUpdated)
        {
            streams = RemoveIgnoredStreams(streams);
            streams = RemoveDuplicates(streams);
        }

        Logger.LogInformation($"Processing M3U streams took {sw.Elapsed.TotalSeconds} seconds");
        return streams;
    }

    private bool ShouldUpdate(M3UFile m3uFile)
    {
        return m3uFile.LastWrite() >= m3uFile.LastUpdated;
    }

    private async Task ProcessAndUpdateStreams(M3UFile m3uFile, List<VideoStream> streams, CancellationToken cancellationToken)
    {
        Stopwatch sw = Stopwatch.StartNew();

        List<VideoStream> existing = Repository.VideoStream.GetVideoStreamsByM3UFileId(m3uFile.Id).ToList();
        existingChannels = new ThreadSafeIntList(m3uFile.StartingChannelNumber < 1 ? 1 : m3uFile.StartingChannelNumber);

        List<ChannelGroup> groups = Repository.ChannelGroup.GetAllChannelGroups().ToList();
        nextchno = m3uFile.StartingChannelNumber < 0 ? 0 : m3uFile.StartingChannelNumber;

        ConcurrentBag<VideoStream> toWrite = new();
        ProcessStreamsConcurrently(streams, existing, groups, m3uFile, toWrite);

        Repository.VideoStream.BulkInsert(toWrite.ToArray());

        m3uFile.LastUpdated = DateTime.Now;
        if (m3uFile.StationCount != streams.Count)
        {
            m3uFile.StationCount = streams.Count;
        }
        Repository.M3UFile.UpdateM3UFile(m3uFile);

        await Repository.SaveAsync().ConfigureAwait(false);

        Logger.LogInformation($"Processing and updating streams took {sw.Elapsed.TotalSeconds} seconds");
    }
    private void ProcessStreamsConcurrently(List<VideoStream> streams, List<VideoStream> existing, List<ChannelGroup> groups, M3UFile m3uFile, ConcurrentBag<VideoStream> toWrite)
    {
        int totalCount = streams.Count();
        int processedCount = 0;

        Parallel.ForEach(streams.Select((value, index) => new { value, index }), (stream) =>
        {
            ChannelGroup? group = groups.FirstOrDefault(a => a.Name.ToLower().Equals(stream.value.Tvg_group.ToLower()));
            VideoStream? dbStream = existing.FirstOrDefault(a => a.Id == stream.value.Id);

            if (dbStream != null)
            {
                ProcessExistingStream(stream.value, dbStream, group);
            }
            else
            {
                ProcessNewStream(stream.value, group);
                toWrite.Add(stream.value);
            }

            int currentProgress = Interlocked.Increment(ref processedCount);
            if (currentProgress % 1000 == 0) // For logging every 1000 items
            {
                Logger.LogInformation($"Processed {currentProgress}/{totalCount} streams, adding {toWrite.Count}");
            }
        });
    }

    private async Task UpdateChannelGroups(List<VideoStream> streams, CancellationToken cancellationToken)
    {
        Stopwatch sw = Stopwatch.StartNew();

        List<string> newGroups = streams.Where(a => a.User_Tvg_group != null).Select(a => a.User_Tvg_group).Distinct().ToList();
        List<ChannelGroup> channelGroups = await Repository.ChannelGroup.GetAllChannelGroups().ToListAsync(cancellationToken);

        await CreateNewChannelGroups(newGroups, channelGroups, cancellationToken);

        Logger.LogInformation($"Updating channel groups took {sw.Elapsed.TotalSeconds} seconds");
    }

    private async Task CreateNewChannelGroups(List<string> newGroups, List<ChannelGroup> existingGroups, CancellationToken cancellationToken)
    {
        int rank = existingGroups.Any() ? existingGroups.Max(a => a.Rank) + 1 : 1;

        foreach (string? group in newGroups)
        {
            if (!existingGroups.Any(a => a.Name == group))
            {
                await Sender.Send(new CreateChannelGroupRequest(group, rank++, true), cancellationToken).ConfigureAwait(false);
            }
        }
    }


    private async Task NotifyUpdates(M3UFile m3uFile, CancellationToken cancellationToken)
    {
        List<string> m3uChannelGroupNames = await Repository.M3UFile.GetChannelGroupNamesFromM3UFile(m3uFile.Id);
        List<ChannelGroup> channelGroups = await Repository.ChannelGroup.GetChannelGroupsFromNames(m3uChannelGroupNames);

        await Sender.Send(new UpdateChannelGroupCountsRequest(channelGroups.Select(a => a.Id)), cancellationToken).ConfigureAwait(false);
        Publisher.Publish(new M3UFileProcessedEvent(), cancellationToken).ConfigureAwait(false);
    }

    private List<VideoStream> RemoveIgnoredStreams(List<VideoStream> streams)
    {
        Setting setting = FileUtil.GetSetting();

        if (setting.NameRegex.Any())
        {
            foreach (string regex in setting.NameRegex)
            {
                List<VideoStream> toIgnore = ListHelper.GetMatchingProperty(streams, "Tvg_name", regex);
                streams.RemoveAll(toIgnore.Contains);
            }
        }

        return streams;
    }

    private List<VideoStream> RemoveDuplicates(List<VideoStream> streams)
    {

        List<string> ids = streams.Select(a => a.Id).Distinct().ToList();
        List<VideoStream> dupes = Repository.VideoStream.FindByCondition(a => ids.Contains(a.Id)).ToList();

        if (dupes.Any())
        {
            List<string> dupeIds = dupes.Select(a => a.Id).Distinct().ToList();

            LogDuplicatesToCSV(dupes);
            streams = streams.Where(a => !dupeIds.Contains(a.Id)).ToList();
        }

        return streams;
    }

    private void LogDuplicatesToCSV(List<VideoStream> dupes)
    {
        string fileName = $"dupes.csv";
        List<string> lines = new() { VideoStream.GetCsvHeader() };
        lines.AddRange(dupes.Select(a => a.ToString()));

        using StreamWriter file = new(fileName);
        foreach (string line in lines)
        {
            file.WriteLine(line);
        }

        Logger.LogError($"Found duplicate streams. Details logged to {fileName}");
    }

    //private async Task AddChannelGroupsFromStreams(IEnumerable<VideoStream> streams, CancellationToken cancellationToken)
    //{
    //    Stopwatch sw = Stopwatch.StartNew();
    //    Logger.LogInformation($"AddChannelGroupsFromStreams started");
    //    List<string> newGroups = streams.Where(a => a.User_Tvg_group != null).Select(a => a.User_Tvg_group).Distinct().ToList();
    //    List<ChannelGroup> channelGroups = await Repository.ChannelGroup.GetAllChannelGroups().ToListAsync(cancellationToken);
    //    int rank = channelGroups.Any() ? channelGroups.Max(a => a.Rank) + 1 : 1;

    //    List<string> newGroupNames = new();

    //    foreach (string? ng in newGroups)
    //    {
    //        if (!channelGroups.Any(a => a.Name == ng))
    //        {

    //            await Sender.Send(new CreateChannelGroupRequest(ng, rank++), cancellationToken).ConfigureAwait(false);
    //        }
    //    }

    //    if (await Repository.SaveAsync().ConfigureAwait(false) > 0)
    //    {
    //        //await Sender.Send(new UpdateChannelGroupCountsRequest(newGroupNames), cancellationToken).ConfigureAwait(false);
    //        await Publisher.Publish(new AddChannelGroupsEvent(), cancellationToken).ConfigureAwait(false);
    //    }
    //    sw.Stop();
    //    string speed = sw.ElapsedMilliseconds.ToString("F3");
    //    Logger.LogInformation($"AddChannelGroupsFromStreams took {speed} seconds");
    //}

    private void ProcessExistingStream(VideoStream stream, VideoStream dbStream, ChannelGroup? group)
    {
        if (group != null)
        {
            dbStream.IsHidden = dbStream.IsHidden || group.IsHidden;
        }

        dbStream.User_Tvg_group = dbStream.Tvg_group == dbStream.User_Tvg_group ? stream.Tvg_group : dbStream.User_Tvg_group;
        dbStream.Tvg_group = stream.Tvg_group;

        if (stream.Tvg_chno == 0 || existingChannels.ContainsInt(stream.Tvg_chno))
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
    }


    private void ProcessNewStream(VideoStream stream, ChannelGroup? group)
    {
        stream.IsHidden = group?.IsHidden ?? false;

        if (stream.User_Tvg_chno == 0 || existingChannels.ContainsInt(stream.Tvg_chno))
        {
            nextchno = existingChannels.GetNextInt(nextchno);
            stream.User_Tvg_chno = nextchno;
            stream.Tvg_chno = nextchno;
        }
    }

}