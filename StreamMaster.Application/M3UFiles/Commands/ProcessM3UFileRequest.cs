using FluentValidation;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Application.ChannelGroups.Commands;

using System.Collections.Concurrent;
using System.Diagnostics;

namespace StreamMaster.Application.M3UFiles.Commands;

public record ProcessM3UFileRequest(int Id, bool? OverWriteChannels = null, bool forceRun = false) : IRequest<M3UFile?> { }

public class ProcessM3UFileRequestValidator : AbstractValidator<ProcessM3UFileRequest>
{
    public ProcessM3UFileRequestValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}


public class ProcessM3UFileRequestHandler(ILogger<ProcessM3UFileRequest> logger, IRepositoryWrapper repository, IJobStatusService jobStatusService, IPublisher publisher, ISender sender, IMemoryCache memoryCache) : IRequestHandler<ProcessM3UFileRequest, M3UFile?>
{
    private SimpleIntList existingChannels = new(0);

    [LogExecutionTimeAspect]
    public async Task<M3UFile?> Handle(ProcessM3UFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            M3UFile? m3uFile = await repository.M3UFile.GetM3UFileByTrackedId(request.Id).ConfigureAwait(false);
            if (m3uFile == null)
            {
                logger.LogCritical("Could not find M3U file");
                return null;
            }

            (List<VideoStream>? streams, int streamCount) = await ProcessStreams(m3uFile).ConfigureAwait(false);
            if (streams == null)
            {
                logger.LogCritical("Error while processing M3U file, bad format");
                return null;
            }

            if (!request.forceRun && !ShouldUpdate(m3uFile, m3uFile.VODTags, request.OverWriteChannels))
            {
                return m3uFile;
            }


            await ProcessAndUpdateStreams(m3uFile, streams, streamCount).ConfigureAwait(false);
            await UpdateChannelGroups(streams, cancellationToken).ConfigureAwait(false);

            await publisher.Publish(new M3UFileProcessedEvent(), cancellationToken).ConfigureAwait(false);

            return m3uFile;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error while processing M3U file");
            return null;
        }
        finally
        {
            jobStatusService.SetM3UIsRunning(false);
        }
    }

    [LogExecutionTimeAspect]
    private async Task<(List<VideoStream>? streams, int streamCount)> ProcessStreams(M3UFile m3uFile)
    {
        Stopwatch sw = Stopwatch.StartNew();

        List<VideoStream>? streams = await m3uFile.GetM3U(logger, CancellationToken.None).ConfigureAwait(false);

        int streamsCount = 0;
        if (streams != null)
        {
            streamsCount = GetRealStreamCount(streams);
            streams = await RemoveIgnoredStreams(streams);
            streams = RemoveDuplicates(streams);
        }

        logger.LogInformation($"Processing M3U {streamsCount}, streams took {sw.Elapsed.TotalSeconds} seconds");
        return (streams, streamsCount);
    }

    private static bool ShouldUpdate(M3UFile m3uFile, List<string> VODTags, bool? OverWriteChannels)
    {
        if (VODTags.Count > 0)
        {
            return true;
        }

        if (OverWriteChannels is not null)
        {
            return true;
        }

        M3UFile? json = m3uFile.ReadJSON();
        return json is null || m3uFile.OverwriteChannelNumbers != json.OverwriteChannelNumbers || m3uFile.LastWrite() >= m3uFile.LastUpdated;
    }



    [LogExecutionTimeAspect]
    private async Task ProcessAndUpdateStreams(M3UFile m3uFile, List<VideoStream> streams, int streamCount)
    {
        await RemoveMissing(streams, m3uFile.Id);

        List<VideoStream> existing = await repository.VideoStream.GetVideoStreamQuery().Where(a => a.M3UFileId == m3uFile.Id).ToListAsync().ConfigureAwait(false);
        existingChannels = new SimpleIntList(m3uFile.StartingChannelNumber < 0 ? 0 : m3uFile.StartingChannelNumber);

        List<ChannelGroup> groups = await repository.ChannelGroup.GetChannelGroups();

        ProcessStreamsConcurrently(streams, existing, groups, m3uFile);

        //_ = await Repository.SaveAsync().ConfigureAwait(false);

        m3uFile.LastUpdated = DateTime.Now;
        if (m3uFile.StationCount != streamCount)
        {
            m3uFile.StationCount = streamCount;
        }

        repository.M3UFile.UpdateM3UFile(m3uFile);
        _ = await repository.SaveAsync().ConfigureAwait(false);
    }

    private async Task RemoveMissing(List<VideoStream> streams, int m3uFileId)
    {
        List<string> streamIds = streams.Select(a => a.Id).ToList();

        IQueryable<VideoStream> toDelete = repository.VideoStream.FindByCondition(a => a.M3UFileId == m3uFileId && !streamIds.Contains(a.Id));
        if (toDelete.Any())
        {
            List<string> ids = [.. toDelete.Select(a => a.Id)];

            IQueryable<VideoStreamLink> toVideoStreamLinkDel = repository.VideoStreamLink.FindByCondition(a => ids.Contains(a.ChildVideoStreamId) || ids.Contains(a.ParentVideoStreamId));
            if (toVideoStreamLinkDel.Any())
            {
                await repository.VideoStreamLink.BulkDeleteAsync(toVideoStreamLinkDel);
            }

            IQueryable<StreamGroupVideoStream> toStreamGroupVideoStreamDel = repository.StreamGroupVideoStream.FindByCondition(a => ids.Contains(a.ChildVideoStreamId));
            if (toStreamGroupVideoStreamDel.Any())
            {
                await repository.StreamGroupVideoStream.BulkDeleteAsync(toStreamGroupVideoStreamDel);
            }

            await repository.VideoStream.BulkDeleteAsync(toDelete);
        }
    }

    private void ProcessStreamsConcurrently(List<VideoStream> streams, List<VideoStream> existing, List<ChannelGroup> groups, M3UFile m3uFile)
    {
        int totalCount = streams.Count;
        Dictionary<string, VideoStream> existingLookup = existing.ToDictionary(a => a.Id, a => a);
        Dictionary<string, ChannelGroup> groupLookup = groups.ToDictionary(g => g.Name, g => g);
        ConcurrentDictionary<string, bool> processed = new();

        ConcurrentBag<VideoStream> toWrite = [];
        ConcurrentBag<VideoStream> toUpdate = [];

        bool overwriteChannelNumbers = m3uFile.OverwriteChannelNumbers;

        int processedCount = 0;

        _ = Parallel.ForEach(streams.Select((stream, index) => (stream, index)), tuple =>
        {
            VideoStream stream = tuple.stream;
            int index = tuple.index;

            if (processed.TryAdd(stream.Id, true))
            {
                _ = groupLookup.TryGetValue(stream.Tvg_group, out ChannelGroup? group);

                if (!existingLookup.TryGetValue(stream.Id, out VideoStream? existingStream))
                {
                    ProcessNewStream(stream, group?.IsHidden ?? false, m3uFile.Name, overwriteChannelNumbers, index);
                    toWrite.Add(stream);
                }
                else
                {
                    if (ProcessExistingStream(stream, existingStream, m3uFile.Name, overwriteChannelNumbers, index))
                    {
                        existingStream.M3UFileId = m3uFile.Id;
                        toUpdate.Add(existingStream);
                    }
                }

                _ = Interlocked.Increment(ref processedCount);
                if (processedCount % 20000 == 0)
                {
                    logger.LogInformation($"Processed {processedCount}/{totalCount} streams, adding {toWrite.Count}, updating: {toUpdate.Count}");
                }
            }
        });


        // Final batch processing
        ProcessBatches(toWrite, toUpdate);
        logger.LogInformation($"Finished Found {processedCount} streams, inserted: {toWrite.Count}, updated: {toUpdate.Count}");
    }

    private void ProcessBatches(ConcurrentBag<VideoStream> toWrite, ConcurrentBag<VideoStream> toUpdate)
    {
        int totalToWrite = toWrite.Count;
        int totalToUpdate = toUpdate.Count;
        int batchWriteCount = 0;
        int batchUpdateCount = 0;

        // Convert to a list and process in chunks
        List<VideoStream> writeList = [.. toWrite];
        List<VideoStream> updateList = toUpdate.ToList();

        int batchSize = 500;

        if (writeList.Count != 0)
        {
            logger.LogInformation($"Inserting {writeList.Count} new streams in the DB");

            for (int i = 0; i < writeList.Count; i += batchSize)
            {

                List<VideoStream> batch = writeList.Skip(i).Take(batchSize).ToList();
                repository.VideoStream.BulkInsert(batch);
                batchWriteCount += batch.Count;

                if (batchWriteCount % 5000 == 0)
                {
                    logger.LogInformation($"Inserted {batchWriteCount}/{totalToWrite} new streams into DB");
                }
            }
        }

        if (updateList.Any())
        {
            logger.LogInformation($"Updating {updateList.Count} streams in DB");
            for (int i = 0; i < updateList.Count; i += batchSize)
            {
                List<VideoStream> batch = updateList.Skip(i).Take(batchSize).ToList();
                repository.VideoStream.BulkUpdate(batch);
                batchUpdateCount += batch.Count;

                if (batchUpdateCount % 5000 == 0)
                {
                    logger.LogInformation($"Updated {batchUpdateCount}/{totalToUpdate} streams in DB");
                }
            }
        }
    }



    private async Task UpdateChannelGroups(List<VideoStream> streams, CancellationToken cancellationToken)
    {
        Stopwatch sw = Stopwatch.StartNew();

        //var badStreams = streams.Where(a => a.User_Tvg_group == null || a.User_Tvg_group == "").ToList();

        List<string> newGroups = streams.Where(a => a.User_Tvg_group is not null and not "").Select(a => a.User_Tvg_group).Distinct().ToList();
        List<ChannelGroup> channelGroups = await repository.ChannelGroup.GetChannelGroups();

        await CreateNewChannelGroups(newGroups, channelGroups, cancellationToken);

        logger.LogInformation($"Updating channel groups took {sw.Elapsed.TotalSeconds} seconds");
    }

    private async Task CreateNewChannelGroups(List<string> newGroups, List<ChannelGroup> existingGroups, CancellationToken cancellationToken)
    {
        foreach (string? group in newGroups)
        {
            if (!existingGroups.Any(a => a.Name == group))
            {
                _ = await sender.Send(new CreateChannelGroupRequest(group, true), cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private async Task<List<VideoStream>> RemoveIgnoredStreams(List<VideoStream> streams)
    {
        Setting setting = memoryCache.GetSetting();
        if (setting.NameRegex.Any())
        {
            foreach (string regex in setting.NameRegex)
            {
                List<VideoStream> toIgnore = ListHelper.GetMatchingProperty(streams, "Tvg_name", regex);
                logger.LogInformation($"Ignoring {toIgnore.Count} streams with regex {regex}");
                _ = streams.RemoveAll(toIgnore.Contains);
            }
        }

        return streams;
    }

    private static int GetRealStreamCount(List<VideoStream> streams)
    {
        List<string> ids = streams.Select(a => a.Id).Distinct().ToList();
        return ids.Count;
    }

    private List<VideoStream> RemoveDuplicates(List<VideoStream> streams)
    {
        List<VideoStream> cleanStreams = streams.GroupBy(s => s.Id)
                  .Select(g => g.First())
                  .ToList();

        logger.LogInformation($"Removed {streams.Count - cleanStreams.Count} duplicate streams");

        return cleanStreams;
    }

    private void LogDuplicatesToCSV(List<VideoStream> dupes)
    {
        string fileName = $"dupes.csv";
        List<string> lines = [VideoStream.GetCsvHeader(), .. dupes.Select(a => a.ToString())];

        using StreamWriter file = new(fileName);
        foreach (string line in lines)
        {
            file.WriteLine(line);
        }

        logger.LogError($"Found duplicate streams. Details logged to {fileName}");
    }

    private bool ProcessExistingStream(VideoStream stream, VideoStream dbStream, string mu3FileName, bool overWriteChannels, int index)
    {

        //Update dbStream
        bool changed = false;

        if (string.IsNullOrEmpty(dbStream.M3UFileName) || dbStream.M3UFileName != mu3FileName)
        {
            changed = true;
            dbStream.M3UFileName = mu3FileName;
        }

        int localNextChno = stream.Tvg_chno;
        if (overWriteChannels)
        {
            localNextChno = overWriteChannels ? existingChannels.GetNextInt(index: index) : existingChannels.GetNextInt(value: stream.Tvg_chno, index: index);
        }

        if (dbStream.Tvg_chno != localNextChno)
        {
            changed = true;
            if (dbStream.Tvg_chno == dbStream.User_Tvg_chno)
            {
                dbStream.User_Tvg_chno = localNextChno;
            }
            if (!overWriteChannels)
            {
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

        if (dbStream.FilePosition != index)
        {
            changed = true;

            dbStream.FilePosition = index;
        }

        return changed;
    }

    //private bool ProcessExistingUserStream(VideoStream stream, VideoStream dbStream, string mu3FileName, bool overWriteChannels, int index)
    //{
    //    bool changed = false;

    //    if (dbStream.User_Tvg_group != stream.Tvg_group)
    //    {
    //        changed = true;
    //        dbStream.User_Tvg_group = stream.Tvg_group;
    //    }

    //    if (string.IsNullOrEmpty(dbStream.M3UFileName) || dbStream.M3UFileName != mu3FileName)
    //    {
    //        changed = true;
    //        dbStream.M3UFileName = mu3FileName;
    //    }


    //    if (overWriteChannels || dbStream.User_Tvg_chno != stream.Tvg_chno)
    //    {
    //        int localNextChno = overWriteChannels ? existingChannels.GetNextInt() : existingChannels.GetNextInt(stream.Tvg_chno);
    //        if (dbStream.User_Tvg_chno != localNextChno)
    //        {
    //            changed = true;
    //            dbStream.User_Tvg_chno = localNextChno;
    //        }
    //    }

    //    if (dbStream.User_Tvg_ID != stream.Tvg_ID)
    //    {
    //        changed = true;
    //        dbStream.User_Tvg_ID = stream.Tvg_ID;
    //    }

    //    if (dbStream.User_Tvg_logo != stream.Tvg_logo)
    //    {
    //        changed = true;

    //        dbStream.User_Tvg_logo = stream.Tvg_logo;
    //    }

    //    if (dbStream.User_Tvg_name != stream.Tvg_name)
    //    {
    //        changed = true;

    //        dbStream.User_Tvg_name = stream.Tvg_name;
    //    }

    //    if (dbStream.FilePosition != index)
    //    {
    //        changed = true;

    //        dbStream.FilePosition = index;
    //    }

    //    return changed;
    //}

    private void ProcessNewStream(VideoStream stream, bool? groupIsHidden, string mu3FileName, bool overWriteChannels, int index)
    {
        if (groupIsHidden is not null)
        {
            stream.IsHidden = (bool)groupIsHidden;
        }

        if (overWriteChannels || stream.User_Tvg_chno == 0 || existingChannels.ContainsInt(stream.Tvg_chno))
        {
            int localNextChno = overWriteChannels ? existingChannels.GetNextInt(index: index) : existingChannels.GetNextInt(value: stream.User_Tvg_chno, index: index);

            stream.User_Tvg_chno = localNextChno;
            stream.Tvg_chno = localNextChno;
        }
        stream.FilePosition = index;
        stream.M3UFileName = mu3FileName;
    }
}