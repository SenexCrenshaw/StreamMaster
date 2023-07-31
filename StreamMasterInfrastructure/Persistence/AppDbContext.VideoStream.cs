using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.VideoStreams;
using StreamMasterApplication.VideoStreams.Commands;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities;
using StreamMasterDomain.Enums;

using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Web;

namespace StreamMasterInfrastructure.Persistence;

public class IconFileDtoComparer : IEqualityComparer<IconFileDto>
{
    public bool Equals(IconFileDto x, IconFileDto y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

        // Assuming Source is the unique identifier for IconFileDto
        return x.Source == y.Source;
    }

    public int GetHashCode(IconFileDto obj)
    {
        if (ReferenceEquals(obj, null)) return 0;

        // Assuming Source is the unique identifier for IconFileDto
        int hashProductName = obj.Source == null ? 0 : obj.Source.GetHashCode();

        return hashProductName;
    }
}


public partial class AppDbContext : IVideoStreamDB
{
    public DbSet<VideoStreamLink> VideoStreamLinks { get; set; }

    public DbSet<VideoStream> VideoStreams { get; set; }

    public async Task AddOrUpdateChildToVideoStreamAsync(string parentVideoStreamId, string childId, int rank, CancellationToken cancellationToken)
    {
        var videoStreamLink = await VideoStreamLinks
            .FirstOrDefaultAsync(vsl => vsl.ParentVideoStreamId == parentVideoStreamId && vsl.ChildVideoStreamId == childId, cancellationToken).ConfigureAwait(false);

        if (videoStreamLink == null)
        {
            videoStreamLink = new VideoStreamLink
            {
                ParentVideoStreamId = parentVideoStreamId,
                ChildVideoStreamId = childId,
                Rank = rank
            };

            await VideoStreamLinks.AddAsync(videoStreamLink, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            videoStreamLink.Rank = rank;
        }

        await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> BuildIconsCacheFromVideoStreams(CancellationToken cancellationToken)
    {
        var streams = VideoStreams
         .Where(a => a.User_Tvg_logo != null && a.User_Tvg_logo.Contains("://"))
         .AsQueryable();

        if (!await streams.AnyAsync(cancellationToken: cancellationToken)) { return false; }

        // For progress reporting
        int totalCount = await streams.CountAsync(cancellationToken: cancellationToken);
        int processedCount = 0;

        Stopwatch processSw = new Stopwatch();
        processSw.Start();

        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount // or any number depending on how much parallelism you want
        };

        var streamsList = await streams.ToListAsync();
        var progressCount = 5000;

        var toWrite= new ConcurrentBag<IconFileDto>();

        Parallel.ForEach(streamsList, parallelOptions, stream =>
        {
            if (cancellationToken.IsCancellationRequested) { return; }

            string source = HttpUtility.UrlDecode(stream.Tvg_logo);
                      
                processSw.Start();
                 var icon = IconHelper.GetIcon(source, stream.User_Tvg_name, stream.M3UFileId,  FileDefinitions.Icon);
            toWrite.Add(icon); ;
            // Report progress for every stream
            var currentProgress = Interlocked.Increment(ref processedCount);
                if (currentProgress % progressCount == 0)
                {
                    processSw.Stop();

                    var percentage = ((double)currentProgress / totalCount * 100).ToString("F2");
                    var speed = processSw.ElapsedMilliseconds.ToString("F3");
                    var avgTimePerItem = processSw.ElapsedMilliseconds / progressCount;
                    int remainingItems = totalCount - currentProgress;

                    double estRemainingTime = (avgTimePerItem * remainingItems) / 1000;

                    _logger.LogInformation($"Progress: {percentage}%, {currentProgress}/{totalCount}, Speed: {speed} ms, ETA: {estRemainingTime} sec");

                    processSw.Restart(); // Reset and start the Stopwatch for the next 1000 items
                }
           
        });

        var icons = _memoryCache.GetIcons(_mapper);
        var missingIcons = toWrite.Except(icons, new IconFileDtoComparer());

        icons.AddRange(missingIcons);
        _memoryCache.Set(icons);

        //totalCount = missingIcons.Count();
        //processedCount = 0;

        //Parallel.ForEach(missingIcons, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, icon =>
        //{
          
        //    icons.Add(icon);

        //    // Report progress for every stream
        //    var currentProgress = Interlocked.Increment(ref processedCount);
        //    if (currentProgress % progressCount == 0)
        //    {
        //        processSw.Stop();

        //        var percentage = ((double)currentProgress / totalCount * 100).ToString("F2");
        //        var speed = processSw.ElapsedMilliseconds.ToString("F3");
        //        var avgTimePerItem = processSw.ElapsedMilliseconds / progressCount;
        //        int remainingItems = totalCount - currentProgress;

        //        double estRemainingTime = (avgTimePerItem * remainingItems) / 1000;

        //        _logger.LogInformation($"Progress: {percentage}%, {currentProgress}/{totalCount}, Speed: {speed} ms, ETA: {estRemainingTime} sec");

        //        processSw.Restart(); // Reset and start the Stopwatch for the next 1000 items
        //    }
        //});

        //_memoryCache.Set(icons);
        return true;

    }

    public async Task<bool> CacheIconsFromVideoStreams(CancellationToken cancellationToken)
    {
        if (!await BuildIconsCacheFromVideoStreams(cancellationToken).ConfigureAwait(false))
        {
            return false;
        }

        return true;
    }

    public async Task<bool> DeleteVideoStreamAsync(string videoStreamId, CancellationToken cancellationToken)
    {
        // Get the VideoStream
        var videoStream = await VideoStreams.FindAsync(new object[] { videoStreamId }, cancellationToken).ConfigureAwait(false);
        if (videoStream == null)
        {
            return false;
        }

        // Remove associated VideoStreamLinks where the VideoStream is a parent
        var parentLinks = await VideoStreamLinks
            .Where(vsl => vsl.ParentVideoStreamId == videoStreamId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        VideoStreamLinks.RemoveRange(parentLinks);

        // Remove associated VideoStreamLinks where the VideoStream is a child
        var childLinks = await VideoStreamLinks
            .Where(vsl => vsl.ChildVideoStreamId == videoStreamId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        VideoStreamLinks.RemoveRange(childLinks);

        // Remove the VideoStream
        VideoStreams.Remove(videoStream);

        // Save changes
        try
        {
            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<VideoStream>> DeleteVideoStreamsByM3UFiledId(int M3UFileId, CancellationToken cancellationToken)
    {
        var streams = VideoStreams.Where(a => a.M3UFileId == M3UFileId).ToList();

        foreach (var stream in streams)
        {
            await DeleteVideoStreamAsync(stream.Id, cancellationToken).ConfigureAwait(false);
        }

        return streams;
    }

    public async Task<List<VideoStream>> GetAllVideoStreamsWithChildrenAsync(CancellationToken cancellationToken)
    {
        return await VideoStreams
            .Include(vs => vs.ChildVideoStreams)
            .ThenInclude(vsl => vsl.ChildVideoStream)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<string> GetAvailableID()
    {
        var id = IdConverter.GetID();
        while (await VideoStreams.AnyAsync(a => a.Id == id))
        {
            id = IdConverter.GetID();
        }

        return id;
    }

    public async Task<List<VideoStream>> GetChildVideoStreamsAsync(string parentId, CancellationToken cancellationToken)
    {
        return await VideoStreamLinks
            .Where(vsl => vsl.ParentVideoStreamId == parentId)
            .Select(vsl => vsl.ChildVideoStream)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<VideoStream>> GetChildVideoStreamsAsync(string parentId)
    {
        return await VideoStreamLinks
            .Where(vsl => vsl.ParentVideoStreamId == parentId)
            .Select(vsl => vsl.ChildVideoStream)
            .ToListAsync().ConfigureAwait(false);
    }

    public M3UFileIdMaxStream? GetM3UFileIdMaxStreamFromUrl(string Url)
    {
        var videoStream = VideoStreams.FirstOrDefault(a => a.User_Url == Url);

        if (videoStream == null)
        {
            return null;
        }

        if (videoStream.M3UFileId == 0)
        {
            return new M3UFileIdMaxStream { M3UFileId = videoStream.M3UFileId, MaxStreams = 999 };
        }

        var m3uFile = M3UFiles.Single(a => a.Id == videoStream.M3UFileId);

        return new M3UFileIdMaxStream { M3UFileId = videoStream.M3UFileId, MaxStreams = m3uFile.MaxStreamCount };
    }

    public async Task<List<StreamGroupDto>> GetStreamGroupsByVideoStreamIdsAsync(List<string> videoStreamIds, string url, CancellationToken cancellationToken)
    {
        var sgs = await GetStreamGroupDtos(url, cancellationToken);

        var matchingStreamGroups = sgs
            .Where(sg => sg.ChildVideoStreams.Any(sgvs => videoStreamIds.Contains(sgvs.Id)))
            .ToList();

        return matchingStreamGroups;
    }

    public async Task<(VideoStreamHandlers videoStreamHandler, List<ChildVideoStreamDto> childVideoStreamDtos)?> GetStreamsFromVideoStreamById(string videoStreamId, CancellationToken cancellationToken = default)
    {
        var videoStream = await GetVideoStreamWithChildrenAsync(videoStreamId, cancellationToken).ConfigureAwait(false);
        if (videoStream == null)
        {
            return null;
        }

        if (!videoStream.ChildVideoStreams.Any())
        {
            var childVideoStreamDto = _mapper.Map<ChildVideoStreamDto>(videoStream);
            var result = GetM3UFileIdMaxStreamFromUrl(childVideoStreamDto.User_Url);
            if (result == null)
            {
                return null;
            }
            childVideoStreamDto.MaxStreams = result.MaxStreams;
            childVideoStreamDto.M3UFileId = result.M3UFileId;
            return (videoStream.VideoStreamHandler, new List<ChildVideoStreamDto> { childVideoStreamDto });
        }

        var childVideoStreams = videoStream.ChildVideoStreams.OrderBy(a => a.Rank).Select(a => a.ChildVideoStream).ToList();
        var childVideoStreamDtos = _mapper.Map<List<ChildVideoStreamDto>>(childVideoStreams);

        foreach (var childVideoStreamDto in childVideoStreamDtos)
        {
            var result = GetM3UFileIdMaxStreamFromUrl(childVideoStreamDto.User_Url);
            if (result == null)
            {
                return null;
            }
            childVideoStreamDto.M3UFileId = result.M3UFileId;
            childVideoStreamDto.MaxStreams = result.MaxStreams;
        }

        return (videoStream.VideoStreamHandler, childVideoStreamDtos);
    }

    public async Task<VideoStreamDto> GetVideoStreamDto(string videoStreamId, CancellationToken cancellationToken)
    {
        var stream = await GetVideoStreamWithChildrenAsync(videoStreamId, cancellationToken).ConfigureAwait(false);

        VideoStreamDto videoStreamDto = _mapper.Map<VideoStreamDto>(stream);

        return videoStreamDto;
    }

    public async Task<List<VideoStreamDto>> GetVideoStreamsDto(CancellationToken cancellationToken)
    {
        var streams = await GetAllVideoStreamsWithChildrenAsync(cancellationToken).ConfigureAwait(false);

        var videoStreamDtos = _mapper.Map<List<VideoStreamDto>>(streams);

        return videoStreamDtos;
    }

    public async Task<List<VideoStream>> GetVideoStreamsForParentAsync(string parentVideoStreamId, CancellationToken cancellationToken)
    {
        return await VideoStreamLinks
            .Where(vsl => vsl.ParentVideoStreamId == parentVideoStreamId)
            .Select(vsl => vsl.ChildVideoStream)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<VideoStream> GetVideoStreamWithChildrenAsync(string videoStreamId, CancellationToken cancellationToken)
    {
        return await VideoStreams
            .Include(vs => vs.ChildVideoStreams)
                .ThenInclude(vsl => vsl.ChildVideoStream)
            .SingleOrDefaultAsync(vs => vs.Id == videoStreamId, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveNonExistingVideoStreamLinksAsync(string parentVideoStreamId, List<ChildVideoStreamDto> existingVideoStreamLinks, CancellationToken cancellationToken)
    {
        var existingLinkIds = existingVideoStreamLinks.Select(vsl => vsl.Id).ToList();

        var linksToRemove = await VideoStreamLinks
            .Where(vsl => !existingLinkIds.Contains(vsl.ChildVideoStreamId) && vsl.ParentVideoStreamId == parentVideoStreamId)
            .ToListAsync(cancellationToken);

        VideoStreamLinks.RemoveRange(linksToRemove);

        await SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SynchronizeChildRelationships(VideoStream videoStream, List<ChildVideoStreamDto> childVideoStreams, CancellationToken cancellationToken)
    {
        bool isChanged = false;
        try
        {
            foreach (var ch in childVideoStreams)
            {
                await AddOrUpdateChildToVideoStreamAsync(videoStream.Id, ch.Id, ch.Rank, cancellationToken).ConfigureAwait(false);
            }

            await RemoveNonExistingVideoStreamLinksAsync(videoStream.Id, childVideoStreams.ToList(), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Handle the exception or log the error
            throw;
        }
        return isChanged;
    }

    public async Task<VideoStreamDto?> UpdateVideoStreamAsync(UpdateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        var videoStream = await GetVideoStreamWithChildrenAsync(request.Id, cancellationToken).ConfigureAwait(false);

        if (videoStream == null)
        {
            return null;
        }
        Setting setting = FileUtil.GetSetting();

        UpdateVideoStream(videoStream, request);
        bool epglogo = false;

        if (request.Tvg_name != null && videoStream.User_Tvg_name != request.Tvg_name)
        {
            videoStream.User_Tvg_name = request.Tvg_name;
            if (setting.EPGAlwaysUseVideoStreamName)
            {
                var test = _memoryCache.GetEPGNameTvgName(videoStream.User_Tvg_name);
                if (test is not null)
                {
                    videoStream.User_Tvg_ID = test;
                }
            }
        }

        if (request.Tvg_ID != null && videoStream.User_Tvg_ID != request.Tvg_ID)
        {
            videoStream.User_Tvg_ID = request.Tvg_ID;
            if (setting.VideoStreamAlwaysUseEPGLogo && videoStream.User_Tvg_ID != null)
            {
                var logoUrl = _memoryCache.GetEPGChannelByTvgId(videoStream.User_Tvg_ID);
                if (logoUrl != null)
                {
                    videoStream.User_Tvg_logo = logoUrl;
                    epglogo = true;
                }
            }
        }

        if (!epglogo && request.Tvg_logo != null && videoStream.User_Tvg_logo != request.Tvg_logo)
        {
            if (request.Tvg_logo == "")
            {
                videoStream.User_Tvg_logo = "";
            }
            else
            {
                List<IconFileDto> icons = _memoryCache.GetIcons(_mapper);
                if (icons.Any(a => a.Source == request.Tvg_logo))
                {
                    videoStream.User_Tvg_logo = request.Tvg_logo;
                }
            }
        }

        _ = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (request.ChildVideoStreams != null)
        {
            await SynchronizeChildRelationships(videoStream, request.ChildVideoStreams, cancellationToken).ConfigureAwait(false);
        }

        _ = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        var ret = _mapper.Map<VideoStreamDto>(videoStream);

        if (videoStream.ChildVideoStreams.Any())
        {
            var ids = videoStream.ChildVideoStreams.Select(a => a.ChildVideoStreamId).ToList();
            var streams = VideoStreams.Where(a => ids.Contains(a.Id)).ToList();
            var dtoStreams = _mapper.Map<List<ChildVideoStreamDto>>(streams);

            foreach (var stream in videoStream.ChildVideoStreams)
            {
                var toUpdate = dtoStreams.SingleOrDefault(a => a.Id == stream.ChildVideoStreamId);

                if (toUpdate == null)
                {
                    var goodCV = videoStream.ChildVideoStreams.SingleOrDefault(a => a.ChildVideoStreamId == stream.ChildVideoStreamId);
                    toUpdate = _mapper.Map<ChildVideoStreamDto>(goodCV);
                    dtoStreams.Add(toUpdate);
                }

                toUpdate.Rank = stream.Rank;
            }

            ret.ChildVideoStreams = dtoStreams;
        }

        return ret;
    }

    private static bool UpdateVideoStream(VideoStream videoStream, VideoStreamUpdate update)
    {
        bool isChanged = false;

        if (update.IsActive != null && videoStream.IsActive != update.IsActive) { isChanged = true; videoStream.IsActive = (bool)update.IsActive; }
        if (update.IsDeleted != null && videoStream.IsDeleted != update.IsDeleted) { isChanged = true; videoStream.IsDeleted = (bool)update.IsDeleted; }
        if (update.IsHidden != null && videoStream.IsHidden != update.IsHidden) { isChanged = true; videoStream.IsHidden = (bool)update.IsHidden; }

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
}
