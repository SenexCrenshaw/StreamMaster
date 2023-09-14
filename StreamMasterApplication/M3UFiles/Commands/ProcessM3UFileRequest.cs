using FluentValidation;

using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.ChannelGroups.Commands;

using StreamMasterDomain.Models;

using System.Diagnostics;

namespace StreamMasterApplication.M3UFiles.Commands;

public record ProcessM3UFileRequest(int Id) : IRequest<M3UFile?> { }

public class ProcessM3UFileRequestValidator : AbstractValidator<ProcessM3UFileRequest>
{
    public ProcessM3UFileRequestValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}


[LogExecutionTimeAspect]
public class ProcessM3UFileRequestHandler(ILogger<ProcessM3UFileRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<ProcessM3UFileRequest, M3UFile?>
{
    private SimpleIntList existingChannels;

    public async Task<M3UFile?> Handle(ProcessM3UFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            M3UFile? m3uFile = await repository.M3UFile.GetM3UFileQuery().FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (m3uFile == null)
            {
                Logger.LogCritical("Could not find M3U file");
                return null;
            }

            (List<VideoStream>? streams, int streamCount) = await ProcessStreams(m3uFile).ConfigureAwait(false);
            if (streams == null)
            {
                Logger.LogCritical("Error while processing M3U file, bad format");
                return null;
            }

            if (streams.Count == 0)
            {
                return m3uFile;
            }

            if (!ShouldUpdate(m3uFile))
            {
                return m3uFile;
            }

            await ProcessAndUpdateStreams(m3uFile, streams, streamCount).ConfigureAwait(false);
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

    private async Task<M3UFileDto?> FetchM3UFile(int id)
    {
        return await Repository.M3UFile.GetM3UFileById(id).ConfigureAwait(false);
    }

    private async Task<(List<VideoStream>? streams, int streamCount)> ProcessStreams(M3UFile m3uFile)
    {
        Stopwatch sw = Stopwatch.StartNew();

        List<VideoStream>? streams = await m3uFile.GetM3U().ConfigureAwait(false);

        int streamsCount = 0;
        if (streams != null)
        {
            streamsCount = GetRealStreamCount(streams);
            streams = await RemoveIgnoredStreams(streams);
            streams = RemoveDuplicates(streams);
        }

        Logger.LogInformation($"Processing M3U streams took {sw.Elapsed.TotalSeconds} seconds");
        return (streams, streamsCount);
    }

    private bool ShouldUpdate(M3UFile m3uFile)
    {
        return m3uFile.LastWrite() >= m3uFile.LastUpdated;
    }

    private async Task ProcessAndUpdateStreams(M3UFile m3uFile, List<VideoStream> streams, int streamCount)
    {
        Stopwatch sw = Stopwatch.StartNew();

        List<VideoStream> existing = await Repository.VideoStream.GetVideoStreamQuery().Where(a => a.M3UFileId == m3uFile.Id).ToListAsync().ConfigureAwait(false);
        existingChannels = new SimpleIntList(m3uFile.StartingChannelNumber < 0 ? 0 : m3uFile.StartingChannelNumber - 1);

        List<ChannelGroupDto> groups = await Repository.ChannelGroup.GetChannelGroups();
        //var nextchno = m3uFile.StartingChannelNumber < 0 ? 0 : m3uFile.StartingChannelNumber;

        //List<VideoStream> toWrite = new()
        //List<VideoStream> toUpdate = new();

        await ProcessStreamsConcurrently(streams, existing, groups, m3uFile);

        //if (toWrite.Any())
        //{
        //    Repository.VideoStream.BulkInsert(toWrite.ToArray());
        //}

        //if (toUpdate.Any())
        //{
        //    Repository.VideoStream.BulkUpdate(toUpdate.ToArray());
        //}

        _ = await Repository.SaveAsync().ConfigureAwait(false);

        m3uFile.LastUpdated = DateTime.Now;
        if (m3uFile.StationCount != streamCount)
        {
            m3uFile.StationCount = streamCount;
        }
        Repository.M3UFile.UpdateM3UFile(m3uFile);
        _ = await Repository.SaveAsync().ConfigureAwait(false);

        Logger.LogInformation($"Processing and updating streams took {sw.Elapsed.TotalSeconds} seconds");
    }

    private async Task ProcessStreamsConcurrently(List<VideoStream> streams, List<VideoStream> existing, List<ChannelGroupDto> groups, M3UFile m3uFile)
    {
        int totalCount = streams.Count;
        int processedCount = 0;
        Dictionary<string, VideoStream> existingLookup = existing.ToDictionary(a => a.Id, a => a);
        Dictionary<string, ChannelGroupDto> groupLookup = groups.ToDictionary(g => g.Name, g => g);

        List<VideoStream> toWrite = new();
        List<VideoStream> toUpdate = new();

        foreach (VideoStream stream in streams)
        {
            _ = groupLookup.TryGetValue(stream.Tvg_group, out ChannelGroupDto? group);

            if (!existingLookup.ContainsKey(stream.Id))
            {
                await ProcessNewStream(stream, group?.IsHidden ?? false, m3uFile.Name);
                toWrite.Add(stream);
            }
            else
            {
                VideoStream? dbStream = existingLookup[stream.Id];
                if (await ProcessExistingStream(stream, dbStream, group?.IsHidden ?? false, m3uFile.Name))
                {
                    toUpdate.Add(dbStream);
                }
            }

            processedCount++;
            if (processedCount % 10000 == 0)
            {
                // Logging
                Logger.LogInformation($"Processed {processedCount}/{totalCount} streams, adding {toWrite.Count}, updating: {toUpdate.Count}");

                Repository.VideoStream.BulkInsert(toWrite.ToArray());
                toWrite.Clear();

                Repository.VideoStream.BulkUpdate(toUpdate.ToArray());
                toUpdate.Clear();
            }
        }

        // Handle any remaining items after the loop
        if (toWrite.Any())
        {
            Repository.VideoStream.BulkInsert(toWrite.ToArray());
        }
        if (toUpdate.Any())
        {
            Repository.VideoStream.BulkUpdate(toUpdate.ToArray());
        }
    }


    private async Task UpdateChannelGroups(List<VideoStream> streams, CancellationToken cancellationToken)
    {
        Stopwatch sw = Stopwatch.StartNew();

        List<string> newGroups = streams.Where(a => a.User_Tvg_group != null).Select(a => a.User_Tvg_group).Distinct().ToList();
        List<ChannelGroupDto> channelGroups = await Repository.ChannelGroup.GetChannelGroups();

        await CreateNewChannelGroups(newGroups, channelGroups, cancellationToken);

        Logger.LogInformation($"Updating channel groups took {sw.Elapsed.TotalSeconds} seconds");
    }

    private async Task CreateNewChannelGroups(List<string> newGroups, List<ChannelGroupDto> existingGroups, CancellationToken cancellationToken)
    {

        foreach (string? group in newGroups)
        {
            if (!existingGroups.Any(a => a.Name == group))
            {
                await Sender.Send(new CreateChannelGroupRequest(group, true), cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private async Task NotifyUpdates(M3UFile m3uFile, CancellationToken cancellationToken)
    {
        //List<string> m3uChannelGroupNames = await Repository.M3UFile.GetChannelGroupNamesFromM3UFile(m3uFile.Id);
        //List<ChannelGroup> channelGroups = await Repository.ChannelGroup.GetChannelGroupsFromNames(m3uChannelGroupNames);

        //await Sender.Send(new UpdateChannelGroupCountsRequest(channelGroups.Select(a => a.Id)), cancellationToken).ConfigureAwait(false);
        await Publisher.Publish(new M3UFileProcessedEvent(), cancellationToken).ConfigureAwait(false);
    }

    private async Task<List<VideoStream>> RemoveIgnoredStreams(List<VideoStream> streams)
    {
        Setting setting = await GetSettingsAsync();
        if (setting.NameRegex.Any())
        {
            foreach (string regex in setting.NameRegex)
            {
                List<VideoStream> toIgnore = ListHelper.GetMatchingProperty(streams, "Tvg_name", regex);
                _ = streams.RemoveAll(toIgnore.Contains);
            }
        }

        return streams;
    }

    private int GetRealStreamCount(List<VideoStream> streams)
    {
        List<string> ids = streams.Select(a => a.Id).Distinct().ToList();
        return ids.Count;
    }

    private List<VideoStream> RemoveDuplicates(List<VideoStream> streams)
    {

        List<VideoStream> cleanStreams = streams.GroupBy(s => s.Id)
                  .Select(g => g.First())
                  .ToList();

        //List<VideoStream> dupes = Repository.VideoStream.FindByCondition(a => ids.Contains(a.Id)).ToList();

        //List<VideoStream> duplicateIds = streams.GroupBy(x => x.Id).Where(g => g.Count() > 1).First().ToList();

        //if (ids.Any())
        //{
        //    List<string> dupeIds = dupes.Select(a => a.Id).Distinct().ToList();

        //    //LogDuplicatesToCSV(dupes);
        //    streams = streams.Where(a => !dupeIds.Contains(a.Id)).ToList();
        //}

        return cleanStreams;
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

    private async Task<bool> ProcessExistingStream(VideoStream stream, VideoStream dbStream, bool isHidden, string mu3FileName)
    {
        bool changed = false;
        if (dbStream.IsHidden != isHidden)
        {
            changed = true;
            dbStream.IsHidden = isHidden;
        }

        if (string.IsNullOrEmpty(dbStream.M3UFileName) || dbStream.M3UFileName != mu3FileName)
        {
            changed = true;
            dbStream.M3UFileName = mu3FileName;
        }

        Setting setting = await GetSettingsAsync();

        if (setting.OverWriteM3UChannels || dbStream.Tvg_chno != stream.Tvg_chno)
        {
            int localNextChno = setting.OverWriteM3UChannels ? existingChannels.GetNextInt() : existingChannels.GetNextInt(stream.Tvg_chno);
            if (dbStream.Tvg_chno != localNextChno)
            {
                changed = true;
                if (dbStream.Tvg_chno == dbStream.User_Tvg_chno)
                {
                    dbStream.User_Tvg_chno = localNextChno;
                }
                dbStream.Tvg_chno = localNextChno;
            }
        }

        if (dbStream.Tvg_group != stream.Tvg_group)
        {
            changed = true;
            if (dbStream.Tvg_group == dbStream.User_Tvg_group)
            {
                dbStream.User_Tvg_group = stream.Tvg_group;
            }
            dbStream.Tvg_group = stream.Tvg_group;
        }

        if (dbStream.Tvg_ID != stream.Tvg_ID)
        {
            changed = true;
            if (dbStream.Tvg_ID == dbStream.User_Tvg_ID)
            {
                dbStream.User_Tvg_ID = stream.Tvg_ID;
            }
            dbStream.Tvg_ID = stream.Tvg_ID;
        }

        if (dbStream.Tvg_logo != stream.Tvg_logo)
        {
            changed = true;
            if (dbStream.Tvg_logo == dbStream.User_Tvg_logo)
            {
                dbStream.User_Tvg_logo = stream.Tvg_logo;
            }
            dbStream.Tvg_logo = stream.Tvg_logo;
        }

        if (dbStream.Tvg_name != stream.Tvg_name)
        {
            changed = true;
            if (dbStream.Tvg_name == dbStream.User_Tvg_name)
            {
                dbStream.User_Tvg_name = stream.Tvg_name;
            }
            dbStream.Tvg_name = stream.Tvg_name;
        }

        return changed;
    }

    private async Task<bool> ProcessExistingUserStream(VideoStream stream, VideoStream dbStream, bool isHidden, string mu3FileName)
    {
        bool changed = false;
        //if (dbStream.IsHidden != isHidden)
        //{
        //    changed = true;
        //    dbStream.IsHidden = isHidden;
        //}

        if (dbStream.User_Tvg_group != stream.Tvg_group)
        {
            changed = true;
            dbStream.User_Tvg_group = stream.Tvg_group;
        }

        if (string.IsNullOrEmpty(dbStream.M3UFileName) || dbStream.M3UFileName != mu3FileName)
        {
            changed = true;
            dbStream.M3UFileName = mu3FileName;
        }


        Setting setting = await GetSettingsAsync();
        if (setting.OverWriteM3UChannels || dbStream.User_Tvg_chno != stream.Tvg_chno)
        {
            int localNextChno = setting.OverWriteM3UChannels ? existingChannels.GetNextInt() : existingChannels.GetNextInt(stream.Tvg_chno);
            if (dbStream.User_Tvg_chno != localNextChno)
            {
                changed = true;
                dbStream.User_Tvg_chno = localNextChno;
            }
        }

        if (dbStream.User_Tvg_ID != stream.Tvg_ID)
        {
            changed = true;
            dbStream.User_Tvg_ID = stream.Tvg_ID;
        }

        if (dbStream.User_Tvg_logo != stream.Tvg_logo)
        {
            changed = true;

            dbStream.User_Tvg_logo = stream.Tvg_logo;
        }

        if (dbStream.User_Tvg_name != stream.Tvg_name)
        {
            changed = true;

            dbStream.User_Tvg_name = stream.Tvg_name;
        }

        return changed;
    }


    private async Task ProcessNewStream(VideoStream stream, bool IsHidden, string mu3FileName)
    {
        stream.IsHidden = IsHidden;

        Setting setting = await GetSettingsAsync();

        if (setting.OverWriteM3UChannels || stream.User_Tvg_chno == 0 || existingChannels.ContainsInt(stream.Tvg_chno))
        {
            int localNextChno = setting.OverWriteM3UChannels ? existingChannels.GetNextInt() : existingChannels.GetNextInt(stream.User_Tvg_chno);

            stream.User_Tvg_chno = localNextChno;
            stream.Tvg_chno = localNextChno;
        }
        stream.M3UFileName = mu3FileName;
    }

}