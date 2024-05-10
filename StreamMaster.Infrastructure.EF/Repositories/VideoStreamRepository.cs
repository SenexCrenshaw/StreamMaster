using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Application.ChannelGroups.Queries;
using StreamMaster.Application.EPG.Queries;
using StreamMaster.Application.Icons.Queries;
using StreamMaster.Application.M3UFiles.QueriesOld;
using StreamMaster.Application.SchedulesDirect.Queries;
using StreamMaster.Application.SchedulesDirect.QueriesOld;
using StreamMaster.Application.StreamGroupChannelGroupLinks.Commands;
using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;
using StreamMaster.Infrastructure.EF.Helpers;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.JsonClasses;
using StreamMaster.SchedulesDirect.Domain.Models;

using System.Data;
using System.Diagnostics;
using System.Linq.Dynamic.Core;


namespace StreamMaster.Infrastructure.EF.Repositories;

public class VideoStreamRepository(ILogger<VideoStreamRepository> intLogger, IRepositoryWrapper repository, ISchedulesDirectDataService schedulesDirectDataService, IIconService iconService, IRepositoryContext repositoryContext, IMapper mapper, IOptionsMonitor<Setting> intSettings, ISender sender)
    : RepositoryBase<VideoStream>(repositoryContext, intLogger, intSettings), IVideoStreamRepository
{

    public PagedResponse<VideoStreamDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<VideoStreamDto>(Count());
    }

    /// <summary>
    /// Updates the channel group name associated with specified video streams.
    /// </summary>
    /// <param name="videoStreamIds">The list of video stream IDs to update.</param>
    /// <param name="newName">The new channel group name.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    public async Task UpdateVideoStreamsChannelGroupNames(IEnumerable<string> videoStreamIds, string newName)
    {
        if (videoStreamIds == null || !videoStreamIds.Any())
        {
            logger.LogWarning("UpdateVideoStreamsChannelGroupNames was called with an empty videoStreamIds list.");
            return;
        }

        if (string.IsNullOrWhiteSpace(newName))
        {
            logger.LogWarning("UpdateVideoStreamsChannelGroupNames was called with an empty newName.");
            return;
        }

        try
        {
            // Updating the associated video streams in the database using GetQuery
            await GetQuery(a => videoStreamIds.Contains(a.Id))
                   .ExecuteUpdateAsync(s => s.SetProperty(b => b.User_Tvg_group, newName))
                   .ConfigureAwait(false);
            logger.LogInformation($"Successfully updated channel group name for {videoStreamIds.Count()} video streams to '{newName}'.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error updating channel group name for video streams. New Name: {newName}");
            throw;  // Re-throwing the exception so the caller is aware of the failure.
        }
    }

    public async Task<(VideoStreamHandlers videoStreamHandler, List<VideoStreamDto> childVideoStreamDtos)?> GetStreamsFromVideoStreamById(string videoStreamId, CancellationToken cancellationToken = default)
    {
        VideoStream? videoStream = await GetQuery(a => a.Id == videoStreamId)
            .Include(a => a.ChildVideoStreams)
            .ThenInclude(a => a.ChildVideoStream)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        if (videoStream == null)
        {
            return null;
        }

        VideoStreamDto videoStreamDto = mapper.Map<VideoStreamDto>(videoStream);
        if (videoStream.ChildVideoStreams is null || !videoStream.ChildVideoStreams.Any())
        {

            M3UFileIdMaxStream? result = await sender.Send(new GetM3UFileIdMaxStreamFromUrlQuery(videoStreamDto.User_Url), cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }
            videoStreamDto.MaxStreams = result.MaxStreams;
            videoStreamDto.M3UFileId = result.M3UFileId;
            return (videoStream.VideoStreamHandler, new List<VideoStreamDto> { videoStreamDto });
        }


        List<VideoStream> childVideoStreams =
        [
            .. RepositoryContext.VideoStreamLinks
                    .Include(a => a.ChildVideoStream)
                    .Where(a => a.ParentVideoStreamId == videoStream.Id)
                    .Select(a => a.ChildVideoStream),
        ];

        List<VideoStreamDto> childVideoStreamDtos = mapper.Map<List<VideoStreamDto>>(childVideoStreams);
        if (!string.IsNullOrEmpty(videoStreamDto.User_Url))
        {
            childVideoStreamDtos.Insert(0, videoStreamDto);
        }

        Dictionary<string, M3UFileIdMaxStream?> m3uCache = [];

        foreach (VideoStreamDto childVideoStreamDto in childVideoStreamDtos)
        {
            if (m3uCache.TryGetValue(childVideoStreamDto.User_Url, out M3UFileIdMaxStream? cachedResult) && cachedResult != null)
            {
                childVideoStreamDto.M3UFileId = cachedResult.M3UFileId;
                childVideoStreamDto.MaxStreams = cachedResult.MaxStreams;
                continue;
            }

            M3UFileIdMaxStream? result = await sender.Send(new GetM3UFileIdMaxStreamFromUrlQuery(childVideoStreamDto.User_Url), cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }
            VideoStreamLink? link = videoStream.ChildVideoStreams.FirstOrDefault(a => a.ChildVideoStreamId == childVideoStreamDto.Id);
            if (link is not null)
            {
                childVideoStreamDto.Rank = link.Rank;
            }
            childVideoStreamDto.M3UFileId = result.M3UFileId;
            childVideoStreamDto.MaxStreams = result.MaxStreams;
            m3uCache[childVideoStreamDto.User_Url] = result;
        }

        return (videoStream.VideoStreamHandler, childVideoStreamDtos);
    }

    private void CreateVideoStream(VideoStream VideoStream)
    {
        Create(VideoStream);
    }

    public async Task<VideoStreamDto?> DeleteVideoStreamById(string VideoStreamId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(VideoStreamId))
        {
            throw new ArgumentNullException(nameof(VideoStreamId));
        }

        VideoStream? videoStream = await
            GetQuery(a => a.Id == VideoStreamId, true)
            .Include(a => a.ChildVideoStreams)
            .ThenInclude(a => a.ChildVideoStream)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        if (videoStream == null)
        {
            return null;
        }

        if (videoStream.ChildVideoStreams.Count != 0)
        {
            RepositoryContext.VideoStreamLinks.RemoveRange(videoStream.ChildVideoStreams);
            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        Delete(videoStream);
        logger.LogInformation($"Video Stream with Name {videoStream.User_Tvg_name} was deleted.");
        return mapper.Map<VideoStreamDto>(videoStream);
    }

    public async Task<List<VideoStreamDto>> SetVideoStreamChannelGroupName(string channelGroupName, string newGroupName, CancellationToken cancellationToken)
    {
        await GetQuery(a => a.User_Tvg_group != null && a.User_Tvg_group == channelGroupName)
              .ExecuteUpdateAsync(s => s.SetProperty(b => b.User_Tvg_group, newGroupName), cancellationToken: cancellationToken)
              .ConfigureAwait(false);

        List<VideoStreamDto> videoStreamsToUpdate = await GetQuery(a => a.User_Tvg_group != null && a.User_Tvg_group == newGroupName)
            .AsNoTracking()
            .ProjectTo<VideoStreamDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return videoStreamsToUpdate;
    }

    [LogExecutionTimeAspect]
    public async Task<List<VideoStreamDto>> DeleteVideoStreamsByM3UFiledId(int M3UFileId, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> query = GetQuery(a => a.M3UFileId == M3UFileId);
        //// Get the VideoStreams
        List<string> videoStreamIds = [.. query.Select(vs => vs.Id)];

        if (!query.Any())
        {
            return [];
        }

        // Remove associated VideoStreamLinks where the VideoStream is a parent
        IQueryable<VideoStreamLink> parentLinks = RepositoryContext.VideoStreamLinks.Where(vsl => videoStreamIds.Contains(vsl.ParentVideoStreamId));
        //await PGSQLRepositoryContext.VideoStreamLinks.Where(vsl => videoStreamIds.Contains(vsl.ParentVideoStreamId)).ExecuteDeleteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        await parentLinks.ExecuteDeleteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        //await PGSQLRepositoryContext.BulkDeleteAsync(parentLinks, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Remove associated VideoStreamLinks where the VideoStream is a child
        IQueryable<VideoStreamLink> childLinks = RepositoryContext.VideoStreamLinks.Where(vsl => videoStreamIds.Contains(vsl.ChildVideoStreamId));
        //await PGSQLRepositoryContext.BulkDeleteAsync(childLinks, cancellationToken: cancellationToken).ConfigureAwait(false);
        await childLinks.ExecuteDeleteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);


        IQueryable<StreamGroupVideoStream> streamgroupLinks = RepositoryContext.StreamGroupVideoStreams.Where(vsl => videoStreamIds.Contains(vsl.ChildVideoStreamId));
        // await PGSQLRepositoryContext.BulkDeleteAsync(streamgroupLinks, cancellationToken: cancellationToken).ConfigureAwait(false);
        await streamgroupLinks.ExecuteDeleteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        //await PGSQLRepositoryContext.VideoStreamLinks.Where(vsl => videoStreamIds.Contains(vsl.ChildVideoStreamId)).ExecuteDeleteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        await query.ExecuteDeleteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        //// Save changes
        //try
        //{
        //    _ = await PGSQLRepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        //}
        //catch (Exception)
        //{
        //    // You can decide how to handle exceptions here, for example by
        //    // logging them. In this case, we're simply swallowing the exception.
        //}

        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return await query.ProjectTo<VideoStreamDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    [LogExecutionTimeAspect]
    public async Task<List<string>> DeleteVideoStreamsAsync(IQueryable<VideoStream> videoStreams, CancellationToken cancellationToken)
    {

        if (!videoStreams.Any())
        {
            return [];
        }

        // Get the VideoStreams
        List<string> videoStreamIds = videoStreams.Select(vs => vs.Id).ToList();
        List<string> cgNames = videoStreams.Select(vs => vs.User_Tvg_group).ToList();

        int deletedCount = 0;

        // Remove associated VideoStreamLinks where the VideoStream is a parent
        IQueryable<VideoStreamLink> parentLinks = RepositoryContext.VideoStreamLinks.Where(vsl => videoStreamIds.Contains(vsl.ParentVideoStreamId));
        await RepositoryContext.BulkDeleteAsyncEntities(parentLinks, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Remove associated VideoStreamLinks where the VideoStream is a child
        IQueryable<VideoStreamLink> childLinks = RepositoryContext.VideoStreamLinks.Where(vsl => videoStreamIds.Contains(vsl.ChildVideoStreamId));
        await RepositoryContext.BulkDeleteAsyncEntities(childLinks, cancellationToken: cancellationToken).ConfigureAwait(false);

        IQueryable<StreamGroupVideoStream> streamgroupLinks = RepositoryContext.StreamGroupVideoStreams.Where(vsl => videoStreamIds.Contains(vsl.ChildVideoStreamId));
        await RepositoryContext.BulkDeleteAsyncEntities(streamgroupLinks, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Remove the VideoStreams
        int count = 0;
        int chunkSize = 500;
        int totalCount = videoStreams.Count();
        logger.LogInformation($"Deleting {totalCount} video streams");
        while (count < totalCount)
        {
            // Calculate the size of the next chunk
            int nextChunkSize = Math.Min(chunkSize, totalCount - count);

            int deletedRecords = videoStreams.Take(nextChunkSize).ExecuteDelete();

            count += nextChunkSize;
            logger.LogInformation($"Deleted {count} of {totalCount} video streams");
        }

        deletedCount += videoStreams.Count();

        // Save changes
        try
        {
            _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // You can decide how to handle exceptions here, for example by
            // logging them. In this case, we're simply swallowing the exception.
        }

        foreach (string cgName in cgNames)
        {
            ChannelGroup? cg = await RepositoryContext.ChannelGroups.FirstOrDefaultAsync(a => a.Name == cgName, cancellationToken: cancellationToken).ConfigureAwait(false);
            await sender.Send(new SyncStreamGroupChannelGroupByChannelIdRequest(cg.Id), cancellationToken).ConfigureAwait(false);
        }


        return videoStreamIds;
    }
    public async Task<VideoStreamDto?> DeleteVideoStream(string videoStreamId, CancellationToken cancellationToken)
    {
        List<string> result = await DeleteVideoStreamsAsync(GetQuery(a => a.Id == videoStreamId), cancellationToken).ConfigureAwait(false);
        if (result.Any())
        {
            VideoStreamDto res = mapper.Map<VideoStreamDto>(result.First());
            return res;
        }
        return null;
    }

    public async Task<List<VideoStreamDto>> SetGroupVisibleByGroupName(string channelGroupName, bool isHidden, CancellationToken cancellationToken)
    {
        await GetQuery(a => a.User_Tvg_group != null && a.User_Tvg_group == channelGroupName)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsHidden, isHidden), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        List<VideoStreamDto> videoStreamsToUpdate = await RepositoryContext.VideoStreams
           .Where(a => a.User_Tvg_group != null && a.User_Tvg_group == channelGroupName)
           .AsNoTracking()
           .ProjectTo<VideoStreamDto>(mapper.ConfigurationProvider)
           .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return videoStreamsToUpdate;
    }

    public async Task<bool> SynchronizeChildRelationships(VideoStream videoStream, List<VideoStreamDto> childVideoStreams, CancellationToken cancellationToken)
    {
        bool isChanged = false;
        try
        {
            int rank = 0;
            foreach (VideoStreamDto ch in childVideoStreams)
            {
                await AddOrUpdateChildToVideoStreamAsync(videoStream.Id, ch.Id, rank, cancellationToken).ConfigureAwait(false);
                ++rank;
            }

            await RemoveNonExistingVideoStreamLinksAsync(videoStream.Id, childVideoStreams.ToList(), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Handle the exception or log the error
            throw;
        }
        return isChanged;
    }

    private static bool MergeVideoStream(VideoStream videoStream, VideoStreamBaseRequest update)
    {
        bool isChanged = false;

        //if (update.IsActive != null && videoStream.IsActive != update.IsActive) { isChanged = true; videoStream.IsActive = (bool)update.IsActive; }
        //if (update.IsDeleted != null && videoStream.IsDeleted != update.IsDeleted) { isChanged = true; videoStream.IsDeleted = (bool)update.IsDeleted; }

        // Update object properties
        if (update.Tvg_chno != null && videoStream.User_Tvg_chno != update.Tvg_chno) { isChanged = true; videoStream.User_Tvg_chno = (int)update.Tvg_chno; }
        if (update.Tvg_group != null && videoStream.User_Tvg_group != update.Tvg_group) { isChanged = true; videoStream.User_Tvg_group = update.Tvg_group; }

        if (update.Url != null && videoStream.User_Url != update.Url)
        {
            isChanged = true;
            if (videoStream.Url == "")
            {
                videoStream.Url = update.Url;
            }
            videoStream.User_Url = update.Url;
        }

        return isChanged;
    }

    public async Task<VideoStream?> CreateVideoStreamAsync(CreateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        HashSet<string> shortIds = [.. GetQuery().Select(v => v.ShortId)];

        string ShortId = UniqueHexGenerator.GenerateUniqueHex(shortIds);

        VideoStream videoStream = new()
        {
            Id = IdConverter.GetID(),
            IsUserCreated = true,
            M3UFileName = "CUSTOM",
            ShortId = ShortId,
        };

        videoStream = await UpdateVideoStreamValues(videoStream, request, cancellationToken).ConfigureAwait(false);
        CreateVideoStream(videoStream);

        _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (request.ChildVideoStreams != null)
        {
            _ = await SynchronizeChildRelationships(videoStream, request.ChildVideoStreams, cancellationToken).ConfigureAwait(false);
        }

        DataResponse<ChannelGroupDto?> cg = await sender.Send(new GetChannelGroupByNameRequest(videoStream.User_Tvg_group), cancellationToken).ConfigureAwait(false);
        if (cg.Data is not null)
        {
            await sender.Send(new SyncStreamGroupChannelGroupByChannelIdRequest(cg.Data.Id), cancellationToken).ConfigureAwait(false);
        }

        return videoStream;
    }

    private async Task<VideoStream> UpdateVideoStreamValues(VideoStream videoStream, VideoStreamBaseRequest request, CancellationToken cancellationToken)
    {


        _ = MergeVideoStream(videoStream, request);
        bool epglogo = false;

        if (request.Tvg_name != null && (videoStream.User_Tvg_name != request.Tvg_name || videoStream.IsUserCreated))
        {
            videoStream.User_Tvg_name = request.Tvg_name;
            await SetVideoStreamLogoFromEPG(videoStream, cancellationToken).ConfigureAwait(false);
        }

        if (request.TimeShift.HasValue)
        {
            videoStream.TimeShift = request.TimeShift.Value;
        }

        if (request.GroupTitle != null && videoStream.GroupTitle != request.GroupTitle)
        {
            videoStream.GroupTitle = request.GroupTitle;
        }

        if (request.StationId != null && videoStream.StationId != request.StationId)
        {
            videoStream.StationId = request.StationId;
        }

        if (request.StreamingProxyType != null && videoStream.StreamingProxyType != request.StreamingProxyType)
        {
            videoStream.StreamingProxyType = (StreamingProxyTypes)request.StreamingProxyType;
        }

        if (request.Tvg_ID != null && (videoStream.User_Tvg_ID != request.Tvg_ID || videoStream.IsUserCreated))
        {
            //string? test = _memoryCache.GetEPGChannelNameByDisplayName(request.EPGId);
            videoStream.User_Tvg_ID = request.Tvg_ID;
            if (Settings.VideoStreamAlwaysUseEPGLogo && videoStream.User_Tvg_ID != null)
            {
                epglogo = await SetVideoStreamLogoFromEPG(videoStream, cancellationToken).ConfigureAwait(false);
            }
        }

        if (!epglogo && request.Tvg_logo != null && (videoStream.User_Tvg_logo != request.Tvg_logo || videoStream.IsUserCreated))
        {
            if (request.Tvg_logo == "")
            {
                videoStream.User_Tvg_logo = "";
            }
            else
            {
                List<IconFileDto> icons = iconService.GetIcons();
                if (icons.Any(a => a.Source == request.Tvg_logo))
                {
                    videoStream.User_Tvg_logo = request.Tvg_logo;
                }
            }
        }

        if (request.ToggleVisibility == true)
        {
            videoStream.IsHidden = !videoStream.IsHidden;
        }
        //else if (request.IsHidden != null && (videoStream.IsHidden != request.IsHidden || videoStream.IsUserCreated))
        //{
        //    videoStream.IsHidden = request.IsHidden.Value;
        //}

        return videoStream;
    }

    private async Task RemoveNonExistingVideoStreamLinksAsync(string parentVideoStreamId, List<VideoStreamDto> existingVideoStreamLinks, CancellationToken cancellationToken)
    {
        List<string> existingLinkIds = existingVideoStreamLinks.Select(vsl => vsl.Id).ToList();

        List<VideoStreamLink> linksToRemove = await RepositoryContext.VideoStreamLinks
            .Where(vsl => !existingLinkIds.Contains(vsl.ChildVideoStreamId) && vsl.ParentVideoStreamId == parentVideoStreamId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        RepositoryContext.VideoStreamLinks.RemoveRange(linksToRemove);

        _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }


    private static string NormalizeLogoString(string input)
    {
        // Remove punctuation characters
        string normalized = new(input.Where(c => !char.IsPunctuation(c)).ToArray());

        // Convert to lowercase
        normalized = normalized.ToLower();

        return normalized;
    }

    private static double GetWeightedMatch(string sentence1, string sentence2)
    {
        // Convert sentences to lowercase and remove punctuation
        string normalizedSentence1 = NormalizeLogoString(sentence1);
        string normalizedSentence2 = NormalizeLogoString(sentence2);

        // Split sentences into individual words
        string[] words1 = normalizedSentence1.Split(' ');
        string[] words2 = normalizedSentence2.Split(' ');

        // Calculate the intersection of words between the two sentences
        IEnumerable<string> wordIntersection = words1.Intersect(words2, StringComparer.OrdinalIgnoreCase);

        // Calculate the weighted match
        double weightedMatch = (double)wordIntersection.Count() / words1.Length;

        return weightedMatch;
    }

    internal async Task<List<VideoStreamDto>> AutoMatchIconToStreams(IEnumerable<string> VideoStreamIds, CancellationToken cancellationToken)
    {

        List<IconFileDto> icons = (await sender.Send(new GetIconsRequest())).Data;

        IQueryable<VideoStream> streams = GetQuery(a => VideoStreamIds.Contains(a.Id));

        List<VideoStreamDto> videoStreamDtos = [];

        foreach (VideoStream stream in streams)
        {
            IconFileDto? icon = icons.FirstOrDefault(a => a.Name.Equals(stream.User_Tvg_name, StringComparison.CurrentCultureIgnoreCase));
            if (icon != null)
            {
                stream.User_Tvg_logo = icon.Source;
                Update(stream);
                videoStreamDtos.Add(mapper.Map<VideoStreamDto>(stream));
                continue;
            }

            var topCheckIcon = icons.Where(a => a.Name.ToLower().Contains(stream.User_Tvg_name.ToLower()))
                         .OrderByDescending(a => GetWeightedMatch(stream.User_Tvg_name, a.Name))
                         .Select(a => new { Icon = a, Weight = GetWeightedMatch(stream.User_Tvg_name, a.Name) })
                         .FirstOrDefault();

            if (topCheckIcon != null && topCheckIcon.Weight > 0.5 && stream.User_Tvg_logo != topCheckIcon.Icon.Source)
            {
                stream.User_Tvg_logo = topCheckIcon.Icon.Source;
                Update(stream);
                VideoStreamDto videoStreamDto = mapper.Map<VideoStreamDto>(stream);
                videoStreamDtos.Add(videoStreamDto);
                break;
            }
        }
        return videoStreamDtos;
    }

    private async Task AddOrUpdateChildToVideoStreamAsync(string parentVideoStreamId, string childId, int rank, CancellationToken cancellationToken)
    {
        VideoStreamLink? videoStreamLink = await RepositoryContext.VideoStreamLinks
            .FirstOrDefaultAsync(vsl => vsl.ParentVideoStreamId == parentVideoStreamId && vsl.ChildVideoStreamId == childId, cancellationToken).ConfigureAwait(false);

        if (videoStreamLink == null)
        {
            videoStreamLink = new VideoStreamLink
            {
                ParentVideoStreamId = parentVideoStreamId,
                ChildVideoStreamId = childId,
                Rank = rank
            };

            _ = await RepositoryContext.VideoStreamLinks.AddAsync(videoStreamLink, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            videoStreamLink.Rank = rank;
        }

        _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public IQueryable<VideoStream> GetJustVideoStreams()
    {
        return GetQuery();
    }

    public async Task<List<VideoStreamDto>> GetVideoStreams()
    {
        return await GetQuery().ProjectTo<VideoStreamDto>(mapper.ConfigurationProvider).ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<string>> GetVideoStreamNames()
    {
        return await GetQuery().Select(a => a.User_Tvg_name).Distinct().ToListAsync();
    }

    public async Task<VideoStreamDto?> GetVideoStreamById(string VideoStreamId)
    {
        VideoStream? ret = await GetQuery(c => c.Id == VideoStreamId)
                            .Include(a => a.ChildVideoStreams)
                            .ThenInclude(a => a.ChildVideoStream)
                             .FirstOrDefaultAsync()
                             .ConfigureAwait(false);

        VideoStreamDto a = mapper.Map<VideoStreamDto>(ret);
        return ret != null ? mapper.Map<VideoStreamDto>(ret) : null;
    }

    public async Task<PagedResponse<VideoStreamDto>> GetPagedVideoStreams(VideoStreamParameters Parameters, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> query = GetQuery(Parameters);
        return await query.GetPagedResponseAsync<VideoStream, VideoStreamDto>(Parameters.PageNumber, Parameters.PageSize, mapper)
                          .ConfigureAwait(false);
    }

    public async Task<List<VideoStreamDto>> GetVideoStreamsByM3UFileId(int m3uFileId)
    {
        IQueryable<VideoStream> query = GetQuery(a => a.M3UFileId == m3uFileId);
        return await query.ProjectTo<VideoStreamDto>(mapper.ConfigurationProvider).ToListAsync().ConfigureAwait(false);
    }

    public async Task<VideoStreamDto?> GetVideoStreamsById(string Id)
    {
        VideoStream? ret = await GetQuery(c => c.Id == Id)
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(false);

        return ret != null ? mapper.Map<VideoStreamDto>(ret) : null;
    }

    public async Task<List<VideoStreamDto>> GetVideoStreamsNotHidden()
    {
        IQueryable<VideoStream> query = GetQuery(a => !a.IsHidden);

        return await query.ProjectTo<VideoStreamDto>(mapper.ConfigurationProvider).ToListAsync();
    }

    private void UpdateVideoStream(VideoStream VideoStream)
    {
        Update(VideoStream);
    }

    public async Task<List<string>> DeleteAllVideoStreamsFromParameters(VideoStreamParameters Parameters, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> toDelete = GetQuery(Parameters).Where(a => a.IsUserCreated);
        return await DeleteVideoStreamsAsync(toDelete, cancellationToken).ConfigureAwait(false);
    }

    public async Task<(List<VideoStreamDto> videoStreams, bool updateChannelGroup)> UpdateAllVideoStreamsFromParameters(VideoStreamParameters Parameters, UpdateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        const int batchSize = 1000;

        List<VideoStream> result = await GetQuery(Parameters).AsNoTracking().ToListAsync(cancellationToken: cancellationToken);

        List<string> ids = result.Select(a => a.Id).OrderBy(a => a).ToList();
        for (int i = 0; i < ids.Count; i += batchSize)
        {
            IEnumerable<string> batch = ids.Skip(i).Take(batchSize);

            await RepositoryContext.VideoStreams
                    .Where(a => batch.Contains(a.Id))
                    .ForEachAsync(s => s.IsHidden = !s.IsHidden, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            _ = await RepositoryContext.SaveChangesAsync(cancellationToken);
        }

        _ = await RepositoryContext.SaveChangesAsync(cancellationToken);
        foreach (VideoStream r in result)
        {
            r.IsHidden = !r.IsHidden;
        }
        List<VideoStreamDto> ret = mapper.Map<List<VideoStreamDto>>(result);
        return (ret, ids.Count > 0);
    }

    //public async Task AddVideoStreamToVideoStream(string ParentVideoStreamId, string ChildVideoStreamId, int? Rank, CancellationToken cancellationToken)
    //{
    //    List<VideoStreamLink> childVideoStreamIds = await PGSQLRepositoryContext.VideoStreamLinks.Where(a => a.ParentVideoStreamId == ParentVideoStreamId).OrderBy(a => a.Rank).AsNoTracking().ToListAsync(cancellationToken: cancellationToken);

    //    childVideoStreamIds ??= new();

    //    if (childVideoStreamIds.Any(a => a.ChildVideoStreamId == ChildVideoStreamId))
    //    {
    //        return;
    //    }

    //    int rank = childVideoStreamIds.Count;
    //    if (Rank.HasValue && Rank.Value > 0 && Rank.Value < childVideoStreamIds.Count)
    //    {
    //        rank = Rank.Value;
    //    }

    //    VideoStreamLink newL = new() { ParentVideoStreamId = ParentVideoStreamId, ChildVideoStreamId = ChildVideoStreamId, Rank = rank };
    //    _ = await PGSQLRepositoryContext.VideoStreamLinks.AddAsync(newL, cancellationToken).ConfigureAwait(false);
    //    childVideoStreamIds.Insert(rank, newL);

    //    for (int i = 0; i < childVideoStreamIds.Count; i++)
    //    {
    //        VideoStreamLink? childVideoStreamId = childVideoStreamIds[i];
    //        childVideoStreamId.Rank = i;
    //        _ = PGSQLRepositoryContext.VideoStreamLinks.Update(childVideoStreamId);
    //    }

    //    _ = await PGSQLRepositoryContext.SaveChangesAsync(cancellationToken);
    //}

    public async Task RemoveVideoStreamFromVideoStream(string ParentVideoStreamId, string ChildVideoStreamId, CancellationToken cancellationToken)
    {
        VideoStreamLink? exists = await RepositoryContext.VideoStreamLinks.FirstOrDefaultAsync(a => a.ParentVideoStreamId == ParentVideoStreamId && a.ChildVideoStreamId == ChildVideoStreamId, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (exists != null)
        {
            _ = RepositoryContext.VideoStreamLinks.Remove(exists);
            _ = await RepositoryContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<VideoStreamDto>> SetVideoStreamChannelNumbersFromIds(IEnumerable<string> Ids, bool OverWriteExisting, int StartNumber, string OrderBy, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = GetQuery(a => Ids.Contains(a.Id), OrderBy);
        return await SetVideoStreamChannelNumbers(videoStreams, OverWriteExisting, StartNumber, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<VideoStreamDto>> SetVideoStreamChannelNumbersFromParameters(VideoStreamParameters Parameters, bool OverWriteExisting, int StartNumber, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = GetQuery(Parameters);
        return await SetVideoStreamChannelNumbers(videoStreams, OverWriteExisting, StartNumber, cancellationToken).ConfigureAwait(false);
    }

    private int GetNextNumber(int startNumber, ConcurrentHashSet<int> existingNumbers)
    {
        while (existingNumbers.Contains(startNumber))
        {
            startNumber++;
        }
        return startNumber;
    }

    private async Task<List<VideoStreamDto>> SetVideoStreamChannelNumbers(IQueryable<VideoStream> videoStreams, bool overWriteExisting, int startNumber, CancellationToken cancellationToken)
    {
        ConcurrentHashSet<int> existingNumbers = [];

        if (!overWriteExisting)
        {
            existingNumbers.UnionWith(videoStreams.Select(a => a.User_Tvg_chno).Distinct());
        }

        bool changed = false;

        int number = overWriteExisting ? startNumber - 1 : startNumber;

        foreach (VideoStream? videoStream in videoStreams)
        {
            if (!overWriteExisting && videoStream.User_Tvg_chno != 0)
            {
                continue;
            }

            if (overWriteExisting)
            {
                videoStream.User_Tvg_chno = ++number;
            }
            else
            {
                number = GetNextNumber(number, existingNumbers);
                videoStream.User_Tvg_chno = number;
                _ = existingNumbers.Add(number);
            }
            changed = true;
            UpdateVideoStream(videoStream);
        }

        if (changed)
        {
            _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return await videoStreams.AsNoTracking().ProjectTo<VideoStreamDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        return [];
    }

    public async Task<List<VideoStreamDto>> SetVideoStreamsLogoFromEPGFromIds(IEnumerable<string> Ids, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = GetQuery(a => Ids.Contains(a.Id));
        return await SetVideoStreamsLogoFromEPG(videoStreams, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<VideoStreamDto>> SetVideoStreamsLogoFromEPGFromParameters(VideoStreamParameters Parameters, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = GetQuery(Parameters);
        return await SetVideoStreamsLogoFromEPG(videoStreams, cancellationToken).ConfigureAwait(false);
    }

    private async Task<bool> SetVideoStreamLogoFromEPG(VideoStream videoStream, CancellationToken cancellationToken)
    {
        MxfService? service = await sender.Send(new GetService(videoStream.User_Tvg_ID), cancellationToken).ConfigureAwait(false);
        if (service is null || !service.extras.TryGetValue("logo", out dynamic? value))
        {
            return false;
        }
        StationImage logo = value;

        if (logo.Url != null)
        {
            videoStream.User_Tvg_logo = logo.Url;
        }
        return true;
    }
    private async Task<List<VideoStreamDto>> SetVideoStreamsLogoFromEPG(IQueryable<VideoStream> videoStreams, CancellationToken cancellationToken)
    {
        int ret = 0;
        foreach (VideoStream videoStream in videoStreams.Where(a => !string.IsNullOrEmpty(a.User_Tvg_ID)))
        {
            if (await SetVideoStreamLogoFromEPG(videoStream, cancellationToken))
            {
                ret++;
            }
        }

        if (ret > 0)
        {
            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return await videoStreams.AsNoTracking().ProjectTo<VideoStreamDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        return [];
    }

    public async Task<List<VideoStreamDto>> ReSetVideoStreamsLogoFromIds(IEnumerable<string> Ids, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = GetQuery(a => Ids.Contains(a.Id));
        return await SetVideoStreamsLogo(videoStreams, cancellationToken);
    }

    public async Task<List<VideoStreamDto>> ReSetVideoStreamsLogoFromParameters(VideoStreamParameters Parameters, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = GetQuery(Parameters);
        return await SetVideoStreamsLogo(videoStreams, cancellationToken);
    }

    private async Task<List<VideoStreamDto>> SetVideoStreamsLogo(IQueryable<VideoStream> videoStreams, CancellationToken cancellationToken)
    {
        int ret = 0;
        foreach (VideoStream? videoStream in videoStreams)
        {
            videoStream.User_Tvg_logo = videoStream.Tvg_logo;
            Update(videoStream);
            ret++;
        }

        if (ret > 0)
        {
            _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return await videoStreams.AsNoTracking().ProjectTo<VideoStreamDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        return [];
    }

    public async Task<List<VideoStreamDto>> GetVideoStreamsForChannelGroup(int channelGroupId, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await RepositoryContext.ChannelGroups.FirstOrDefaultAsync(a => a.Id == channelGroupId, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (channelGroup == null)
        {
            return [];
        }

        List<VideoStreamDto> ret = await GetQuery(a => a.User_Tvg_group == channelGroup.Name).AsNoTracking()
            .ProjectTo<VideoStreamDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return ret;
    }

    public async Task<List<VideoStreamDto>> GetVideoStreamsForChannelGroups(IEnumerable<int> channelGroupIds, CancellationToken cancellationToken)
    {
        List<string> channelGroupNames = await RepositoryContext.ChannelGroups.Where(a => channelGroupIds.Contains(a.Id)).Select(a => a.Name).Distinct().ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        List<VideoStreamDto> ret = await GetQuery(a => channelGroupNames.Contains(a.User_Tvg_group)).AsNoTracking()
            .ProjectTo<VideoStreamDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return ret;
    }

    public async Task<List<VideoStreamDto>> SetVideoStreamSetEPGsFromName(IEnumerable<string> VideoStreamIds, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = [];

        foreach (VideoStream? videoStream in GetQuery(a => VideoStreamIds.Contains(a.Id)))
        {
            string? test = await sender.Send(new GetEPGNameTvgName(videoStream.User_Tvg_name), cancellationToken).ConfigureAwait(false);
            if (test is not null && test != videoStream.User_Tvg_ID)
            {
                videoStream.User_Tvg_ID = test;
                Update(videoStream);
                results.Add(mapper.Map<VideoStreamDto>(videoStream));
            }
        }

        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return results;
    }

    public async Task<(VideoStreamDto? videoStream, ChannelGroupDto? updatedChannelGroup)> UpdateVideoStreamAsync(UpdateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        VideoStream? videoStream = await FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (videoStream == null)
        {
            return (null, null);
        }

        string UpdateSGCG = string.Empty;
        if (request.Tvg_group != null && videoStream.User_Tvg_group != request.Tvg_group)
        {
            UpdateSGCG = videoStream.User_Tvg_group;
        }

        bool updateChannelGroup = request.ToggleVisibility == true || !string.IsNullOrEmpty(UpdateSGCG);


        videoStream = await UpdateVideoStreamValues(videoStream, request, cancellationToken).ConfigureAwait(false);
        UpdateVideoStream(videoStream);

        _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (request.ChildVideoStreams != null)
        {
            _ = await SynchronizeChildRelationships(videoStream, request.ChildVideoStreams, cancellationToken).ConfigureAwait(false);
        }

        VideoStreamDto? dto = mapper.Map<VideoStreamDto?>(videoStream);
        DataResponse<ChannelGroupDto?>? cg = await sender.Send(new GetChannelGroupByNameRequest(dto.User_Tvg_group), cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(UpdateSGCG))
        {
            DataResponse<ChannelGroupDto?> origCg = await sender.Send(new GetChannelGroupByNameRequest(UpdateSGCG), cancellationToken).ConfigureAwait(false);
            List<StreamGroupVideoStream> sgvids = RepositoryContext.StreamGroupVideoStreams.Where(a => a.ChildVideoStreamId == videoStream.Id).ToList();
            if (sgvids.Count > 0)
            {
                RepositoryContext.StreamGroupVideoStreams.RemoveRange(sgvids);
                await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            if (cg?.Data is not null)
            {
                await sender.Send(new SyncStreamGroupChannelGroupByChannelIdRequest(cg.Data.Id), cancellationToken).ConfigureAwait(false);
            }
        }

        return (dto, cg.Data);
    }

    public async Task<(List<VideoStreamDto> videoStreams, List<ChannelGroupDto> updatedChannelGroups)> UpdateVideoStreamsAsync(IEnumerable<UpdateVideoStreamRequest> VideoStreamUpdates, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> ret = [];
        bool updateCG = false;
        List<ChannelGroupDto> updatedChannelGroups = [];

        foreach (UpdateVideoStreamRequest request in VideoStreamUpdates)
        {
            (VideoStreamDto? videoStream, ChannelGroupDto? updatedChannelGroup) = await UpdateVideoStreamAsync(request, cancellationToken).ConfigureAwait(false);
            if (videoStream != null)
            {
                ret.Add(videoStream);
            }
            if (!updateCG && updatedChannelGroup != null)
            {
                updatedChannelGroups.Add(updatedChannelGroup);
                updateCG = true;
            }
        }
        return (ret, updatedChannelGroups);
    }

    public IQueryable<VideoStream> GetVideoStreamQuery()
    {
        return GetQuery();
    }

    private async Task<List<VideoStreamDto>> AutoSetEPGs(IQueryable<VideoStream> videoStreams, CancellationToken cancellationToken)
    {

        DataResponse<List<StationChannelName>> stationChannelNamesD = await sender.Send(new GetStationChannelNamesRequest(), cancellationToken).ConfigureAwait(false);
        List<StationChannelName> stationChannelNames = stationChannelNamesD.Data;
        stationChannelNames = stationChannelNames.OrderBy(a => a.Channel).ToList();

        List<string> tomatch = stationChannelNames.Select(a => a.DisplayName).Distinct().ToList();
        string tomatchString = string.Join(',', tomatch);

        List<VideoStreamDto> results = [];

        //foreach (VideoStream videoStream in videoStreams)
        //{
        //    var scoredMatches = stationChannelNames
        //         .Select(p => new
        //         {
        //             Channel = p,
        //             Score = AutoEPGMatch.GetMatchingScore(videoStream.User_Tvg_name, p.Channel)
        //         })
        //         .Where(x => x.Score > 0) // Filter out non-matches
        //         .OrderByDescending(x => x.Score) // Sort by score in descending order
        //         .ToList();

        //    if (!scoredMatches.Any())
        //    {
        //        scoredMatches = stationChannelNames
        //         .Select(p => new
        //         {
        //             Channel = p,
        //             Score = AutoEPGMatch.GetMatchingScore(videoStream.User_Tvg_name, p.DisplayName)
        //         })
        //         .Where(x => x.Score > 0) // Filter out non-matches
        //         .OrderByDescending(x => x.Score).ToList(); // Sort by score in descending order
        //    }

        //    if (scoredMatches.Any())
        //    {
        //        videoStream.User_Tvg_ID = scoredMatches[0].Channel.Channel;
        //        UpdateVideoStream(videoStream);

        //        if (setting.VideoStreamAlwaysUseEPGLogo)
        //        {
        //            await SetVideoStreamLogoFromEPG(videoStream, cancellationToken).ConfigureAwait(false);
        //        }
        //        results.Add(mapper.Map<VideoStreamDto>(videoStream));
        //    }
        //}

        await Parallel.ForEachAsync(videoStreams, async (videoStream, token) =>
        {
            var scoredMatches = stationChannelNames
                .Select(p => new
                {
                    Channel = p,
                    Score = AutoEPGMatch.GetMatchingScore(videoStream.User_Tvg_name, p.Channel)
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ToList();

            if (!scoredMatches.Any())
            {
                scoredMatches = stationChannelNames
                    .Select(p => new
                    {
                        Channel = p,
                        Score = AutoEPGMatch.GetMatchingScore(videoStream.User_Tvg_name, p.DisplayName)
                    })
                    .Where(x => x.Score > 0)
                    .OrderByDescending(x => x.Score)
                    .ToList();
            }

            if (scoredMatches.Any())
            {
                videoStream.User_Tvg_ID = scoredMatches[0].Channel.Channel;
                UpdateVideoStream(videoStream);

                if (Settings.VideoStreamAlwaysUseEPGLogo)
                {
                    await SetVideoStreamLogoFromEPG(videoStream, token).ConfigureAwait(false);
                }
                results.Add(mapper.Map<VideoStreamDto>(videoStream));
            }
        });


        if (results.Any())
        {
            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        return results;
    }




    public async Task<List<VideoStreamDto>> AutoSetEPGFromIds(List<string> ids, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = GetQuery(a => ids.Contains(a.Id));
        return await AutoSetEPGs(videoStreams, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<VideoStreamDto>> AutoSetEPGFromParameters(VideoStreamParameters Parameters, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = GetQuery(Parameters);
        return await AutoSetEPGs(videoStreams, cancellationToken).ConfigureAwait(false);
    }

    private static string GetFirstFourOrBlank(string input)
    {
        return string.IsNullOrEmpty(input) || input.Length < 4 || !IsAllDigits(input[..4]) ? "0000" : input[..4];
    }

    private static bool IsAllDigits(string value)
    {
        foreach (char c in value)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }
        return true;
    }

    public async Task<List<VideoStreamDto>> SetVideoStreamTimeShiftsFromIds(List<string> ids, int timeShift, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = GetQuery(a => ids.Contains(a.Id));
        return await SetVideoStreamTimeShifts(videoStreams, timeShift, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<VideoStreamDto>> SetVideoStreamTimeShiftFromParameters(VideoStreamParameters parameters, int timeShift, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = GetQuery(parameters);
        return await SetVideoStreamTimeShifts(videoStreams, timeShift, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<VideoStreamDto>> SetVideoStreamTimeShifts(IQueryable<VideoStream> videoStreams, int timeShift, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = [];

        foreach (VideoStream? videoStream in videoStreams)
        {
            videoStream.TimeShift = timeShift;// GetFirstFourOrBlank(timeShift);
            Update(videoStream);
            results.Add(mapper.Map<VideoStreamDto>(videoStream));
        }

        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return results;
    }

    public async Task<List<VideoStreamDto>> VideoStreamChangeEPGNumber(int oldEPGNumber, int newEPGNumber, CancellationToken cancellationToken)
    {
        Stopwatch stopWatch = Stopwatch.StartNew();
        logger.LogInformation($"Starting VideoStreamChangeEPGNumber {oldEPGNumber} {newEPGNumber}");

        string starts = $"{oldEPGNumber}-";

        // Using raw SQL for bulk update
        string sql = $"UPDATE VideoStreams SET User_Tvg_ID = REPLACE(User_Tvg_ID, '{starts}', '{newEPGNumber}-') WHERE User_Tvg_ID LIKE '{starts}%'";
        await RepositoryContext.ExecuteSqlRawAsyncEntities(sql, cancellationToken: cancellationToken);


        //var results = await PGSQLRepositoryContext.VideoStreams.Where(a => a.User_Tvg_ID.StartsWith(starts)).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        //foreach (var videoStream in results)
        //{
        //    var (epgNumber, stationId) = epgHelper.ExtractEPGNumberAndStationId(videoStream.User_Tvg_ID);
        //    videoStream.User_Tvg_ID = videoStream.User_Tvg_ID = $"{newEPGNumber}-{stationId}";
        //}

        //await PGSQLRepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        List<VideoStream> results = await RepositoryContext.VideoStreams
    .Where(a => a.User_Tvg_ID.StartsWith($"{newEPGNumber}-"))

    .ToListAsync(cancellationToken: cancellationToken)
    .ConfigureAwait(false);

        schedulesDirectDataService.ChangeServiceEPGNumber(oldEPGNumber, newEPGNumber);
        stopWatch.Stop();
        logger.LogInformation($"Finished VideoStreamChangeEPGNumber processed {results.Count} in {stopWatch.ElapsedMilliseconds} ms");
        return mapper.Map<List<VideoStreamDto>>(results);
    }
}