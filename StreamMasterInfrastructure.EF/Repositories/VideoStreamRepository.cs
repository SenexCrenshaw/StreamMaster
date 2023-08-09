using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.M3UFiles.Queries;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Enums;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

namespace StreamMasterInfrastructureEF.Repositories;

public class VideoStreamRepository : RepositoryBase<VideoStream>, IVideoStreamRepository
{
    private readonly ISortHelper<VideoStream> _VideoStreamSortHelper;
    private readonly IMemoryCache _memoryCache;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public VideoStreamRepository(RepositoryContext repositoryContext, ISortHelper<VideoStream> VideoStreamSortHelper, IMapper mapper, IMemoryCache memoryCache, ISender sender) : base(repositoryContext)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
        _sender = sender;
        _VideoStreamSortHelper = VideoStreamSortHelper;
    }

    public async Task<(VideoStreamHandlers videoStreamHandler, List<ChildVideoStreamDto> childVideoStreamDtos)?> GetStreamsFromVideoStreamById(string videoStreamId, CancellationToken cancellationToken = default)
    {
        VideoStream? videoStream = await GetVideoStreamWithChildrenAsync(videoStreamId, cancellationToken).ConfigureAwait(false);
        if (videoStream == null)
        {
            return null;
        }

        if (!videoStream.ChildVideoStreams.Any())
        {
            ChildVideoStreamDto childVideoStreamDto = _mapper.Map<ChildVideoStreamDto>(videoStream);
            M3UFileIdMaxStream? result = await _sender.Send(new GetM3UFileIdMaxStreamFromUrlQuery(childVideoStreamDto.User_Url), cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }
            childVideoStreamDto.MaxStreams = result.MaxStreams;
            childVideoStreamDto.M3UFileId = result.M3UFileId;
            return (videoStream.VideoStreamHandler, new List<ChildVideoStreamDto> { childVideoStreamDto });
        }

        List<VideoStream> childVideoStreams = videoStream.ChildVideoStreams.OrderBy(a => a.Rank).Select(a => a.ChildVideoStream).ToList();
        List<ChildVideoStreamDto> childVideoStreamDtos = _mapper.Map<List<ChildVideoStreamDto>>(childVideoStreams);

        foreach (ChildVideoStreamDto childVideoStreamDto in childVideoStreamDtos)
        {
            M3UFileIdMaxStream? result = await _sender.Send(new GetM3UFileIdMaxStreamFromUrlQuery(childVideoStreamDto.User_Url), cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }
            childVideoStreamDto.M3UFileId = result.M3UFileId;
            childVideoStreamDto.MaxStreams = result.MaxStreams;
        }

        return (videoStream.VideoStreamHandler, childVideoStreamDtos);
    }

    public void CreateVideoStream(VideoStream VideoStream)
    {
        Create(VideoStream);
    }

    public void DeleteVideoStream(VideoStream VideoStream)
    {
        Delete(VideoStream);
    }

    public async Task SetGroupNameByGroupName(string channelGroupName, string newGroupName, CancellationToken cancellationToken)
    {
        await RepositoryContext.VideoStreams
              .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroupName.ToLower())
              .ExecuteUpdateAsync(s => s.SetProperty(b => b.User_Tvg_group, newGroupName), cancellationToken: cancellationToken)
              .ConfigureAwait(false);
    }

    public async Task<IEnumerable<VideoStream>> DeleteVideoStreamsByM3UFiledId(int M3UFileId, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = RepositoryContext.VideoStreams
          .Where(a => a.M3UFileId == M3UFileId);

        await DeleteVideoStreamsAsync(videoStreams, cancellationToken).ConfigureAwait(false);

        return videoStreams;
    }

    public async Task<int> DeleteVideoStreamsAsync(IEnumerable<VideoStream> videoStreams, CancellationToken cancellationToken)
    {
        // Get the VideoStreams
        List<string> videoStreamIds = videoStreams.Select(vs => vs.Id).ToList();

        if (!videoStreams.Any())
        {
            return 0;
        }

        int deletedCount = 0;

        // Remove associated VideoStreamLinks where the VideoStream is a parent
        List<VideoStreamLink> parentLinks = await RepositoryContext.VideoStreamLinks
            .Where(vsl => videoStreamIds.Contains(vsl.ParentVideoStreamId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        if (parentLinks.Count > 0)
        {
            RepositoryContext.VideoStreamLinks.RemoveRange(parentLinks);
            deletedCount += parentLinks.Count;
        }

        // Remove associated VideoStreamLinks where the VideoStream is a child
        List<VideoStreamLink> childLinks = await RepositoryContext.VideoStreamLinks
            .Where(vsl => videoStreamIds.Contains(vsl.ChildVideoStreamId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        if (childLinks.Count > 0)
        {
            RepositoryContext.VideoStreamLinks.RemoveRange(childLinks);
            deletedCount += childLinks.Count;
        }

        // Remove the VideoStreams
        RepositoryContext.VideoStreams.RemoveRange(videoStreams);
        deletedCount += videoStreams.Count();

        // Save changes
        try
        {
            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // You can decide how to handle exceptions here, for example by
            // logging them. In this case, we're simply swallowing the exception.
        }

        return deletedCount;
    }

    public async Task<bool> DeleteVideoStreamAsync(string videoStreamId, CancellationToken cancellationToken)
    {
        // Get the VideoStream
        VideoStream? videoStream = await RepositoryContext.VideoStreams.FindAsync(new object[] { videoStreamId }, cancellationToken).ConfigureAwait(false);
        if (videoStream == null)
        {
            return false;
        }

        // Remove associated VideoStreamLinks where the VideoStream is a parent
        List<VideoStreamLink> parentLinks = await RepositoryContext.VideoStreamLinks
            .Where(vsl => vsl.ParentVideoStreamId == videoStreamId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        RepositoryContext.VideoStreamLinks.RemoveRange(parentLinks);

        // Remove associated VideoStreamLinks where the VideoStream is a child
        List<VideoStreamLink> childLinks = await RepositoryContext.VideoStreamLinks
            .Where(vsl => vsl.ChildVideoStreamId == videoStreamId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        RepositoryContext.VideoStreamLinks.RemoveRange(childLinks);

        // Remove the VideoStream
        RepositoryContext.VideoStreams.Remove(videoStream);

        // Save changes
        try
        {
            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task SetGroupVisibleByGroupName(string channelGroupName, bool isHidden, CancellationToken cancellationToken)
    {
        await RepositoryContext.VideoStreams
                   .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroupName.ToLower())
        .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsHidden, isHidden), cancellationToken: cancellationToken)
                   .ConfigureAwait(false);
    }

    public async Task<bool> SynchronizeChildRelationships(VideoStream videoStream, List<ChildVideoStreamDto> childVideoStreams, CancellationToken cancellationToken)
    {
        bool isChanged = false;
        try
        {
            foreach (ChildVideoStreamDto ch in childVideoStreams)
            {
                await AddOrUpdateChildToVideoStreamAsync(videoStream.Id, ch.Id, ch.Rank, cancellationToken).ConfigureAwait(false);
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

    private async Task<VideoStream?> GetVideoStreamWithChildrenAsync(string videoStreamId, CancellationToken cancellationToken)
    {
        return await RepositoryContext.VideoStreams
            .Include(vs => vs.ChildVideoStreams)
                .ThenInclude(vsl => vsl.ChildVideoStream)
            .SingleOrDefaultAsync(vs => vs.Id == videoStreamId, cancellationToken).ConfigureAwait(false);
    }

    private static bool MergeVideoStream(VideoStream videoStream, VideoStreamUpdate update)
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

    public async Task<VideoStreamDto?> UpdateVideoStreamAsync(VideoStreamUpdate request, CancellationToken cancellationToken)
    {
        VideoStream videoStream = await GetVideoStreamWithChildrenAsync(request.Id, cancellationToken).ConfigureAwait(false);

        if (videoStream == null)
        {
            return null;
        }
        Setting setting = FileUtil.GetSetting();

        MergeVideoStream(videoStream, request);
        bool epglogo = false;

        if (request.Tvg_name != null && videoStream.User_Tvg_name != request.Tvg_name)
        {
            videoStream.User_Tvg_name = request.Tvg_name;
            if (setting.EPGAlwaysUseVideoStreamName)
            {
                string? test = _memoryCache.GetEPGNameTvgName(videoStream.User_Tvg_name);
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
                string? logoUrl = _memoryCache.GetEPGChannelByTvgId(videoStream.User_Tvg_ID);
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

        UpdateVideoStream(videoStream);

        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (request.ChildVideoStreams != null)
        {
            await SynchronizeChildRelationships(videoStream, request.ChildVideoStreams, cancellationToken).ConfigureAwait(false);
        }

        _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        VideoStreamDto ret = _mapper.Map<VideoStreamDto>(videoStream);

        if (videoStream.ChildVideoStreams.Any())
        {
            List<string> ids = videoStream.ChildVideoStreams.Select(a => a.ChildVideoStreamId).ToList();
            List<VideoStream> streams = RepositoryContext.VideoStreams.Where(a => ids.Contains(a.Id)).ToList();
            List<ChildVideoStreamDto> dtoStreams = _mapper.Map<List<ChildVideoStreamDto>>(streams);

            foreach (VideoStreamLink stream in videoStream.ChildVideoStreams)
            {
                ChildVideoStreamDto? toUpdate = dtoStreams.SingleOrDefault(a => a.Id == stream.ChildVideoStreamId);

                if (toUpdate == null)
                {
                    VideoStreamLink? goodCV = videoStream.ChildVideoStreams.SingleOrDefault(a => a.ChildVideoStreamId == stream.ChildVideoStreamId);
                    toUpdate = _mapper.Map<ChildVideoStreamDto>(goodCV);
                    dtoStreams.Add(toUpdate);
                }

                toUpdate.Rank = stream.Rank;
            }

            ret.ChildVideoStreams = dtoStreams;
        }

        return ret;
    }

    private async Task RemoveNonExistingVideoStreamLinksAsync(string parentVideoStreamId, List<ChildVideoStreamDto> existingVideoStreamLinks, CancellationToken cancellationToken)
    {
        List<string> existingLinkIds = existingVideoStreamLinks.Select(vsl => vsl.Id).ToList();

        List<VideoStreamLink> linksToRemove = await RepositoryContext.VideoStreamLinks
            .Where(vsl => !existingLinkIds.Contains(vsl.ChildVideoStreamId) && vsl.ParentVideoStreamId == parentVideoStreamId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        RepositoryContext.VideoStreamLinks.RemoveRange(linksToRemove);

        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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

            await RepositoryContext.VideoStreamLinks.AddAsync(videoStreamLink, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            videoStreamLink.Rank = rank;
        }

        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public IQueryable<VideoStream> GetAllVideoStreams()
    {
        return FindAll()
             .Include(vs => vs.ChildVideoStreams)
            .ThenInclude(vsl => vsl.ChildVideoStream)
                        .OrderBy(p => p.Id);
    }

    public async Task<VideoStreamDto> GetVideoStreamDtoByIdAsync(string Id, CancellationToken cancellationToken = default)
    {
        VideoStream stream = await GetVideoStreamByIdAsync(Id, cancellationToken).ConfigureAwait(false);

        VideoStreamDto videoStreamDto = _mapper.Map<VideoStreamDto>(stream);

        return videoStreamDto;
    }

    public async Task<VideoStream> GetVideoStreamByIdAsync(string Id, CancellationToken cancellationToken = default)
    {
        VideoStream stream = await GetVideoStreamWithChildrenAsync(Id, cancellationToken).ConfigureAwait(false);

        return stream;
    }

    public IQueryable<VideoStream> GetVideoStreamsChannelGroupName(string channelGroupName)
    {
        IQueryable<VideoStream> streams = GetAllVideoStreams();
        IQueryable<VideoStream> ret = streams.Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroupName.ToLower());

        return ret;
    }

    public IQueryable<VideoStream> GetVideoStreamsByM3UFileId(int m3uFileId)
    {
        IQueryable<VideoStream> streams = GetAllVideoStreams();
        IQueryable<VideoStream> ret = streams.Where(a => a.M3UFileId == m3uFileId);

        return ret;
    }

    public IQueryable<VideoStream> GetVideoStreamsByMatchingIds(IEnumerable<string> ids)
    {
        IQueryable<VideoStream> streams = GetAllVideoStreams();
        IQueryable<VideoStream> ret = streams.Where(a => ids.Contains(a.Id));

        return ret;
    }

    public IQueryable<VideoStream> GetVideoStreamsHidden()
    {
        IQueryable<VideoStream> streams = GetAllVideoStreams();
        IQueryable<VideoStream> ret = streams.Where(a => !a.IsHidden);

        return ret;
    }

    public async Task<PagedResponse<VideoStream>> GetVideoStreamsAsync(VideoStreamParameters videoStreamParameters, CancellationToken cancellationToken)
    {
        return await GetEntitiesAsync<VideoStream>(videoStreamParameters, _mapper);
    }

    public void UpdateVideoStream(VideoStream VideoStream)
    {
        Update(VideoStream);
    }
}