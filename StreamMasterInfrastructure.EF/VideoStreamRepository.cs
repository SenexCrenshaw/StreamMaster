using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Enums;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

namespace StreamMasterInfrastructure.EF;

public class VideoStreamRepository : RepositoryBase<VideoStream>, IVideoStreamRepository
{
    private ISortHelper<VideoStream> _VideoStreamSortHelper;
    private IMemoryCache _memoryCache;
    private IMapper _mapper;
    public VideoStreamRepository(RepositoryContext repositoryContext, ISortHelper<VideoStream> VideoStreamSortHelper, IMapper mapper, IMemoryCache memoryCache) : base(repositoryContext)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
        _VideoStreamSortHelper = VideoStreamSortHelper;
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

    public async Task<VideoStream> GetVideoStreamWithChildrenAsync(string videoStreamId, CancellationToken cancellationToken)
    {
        return await RepositoryContext.VideoStreams
            .Include(vs => vs.ChildVideoStreams)
                .ThenInclude(vsl => vsl.ChildVideoStream)
            .SingleOrDefaultAsync(vs => vs.Id == videoStreamId, cancellationToken).ConfigureAwait(false);
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
    public async Task<VideoStreamDto?> UpdateVideoStreamAsync(VideoStreamUpdate request, CancellationToken cancellationToken)
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

        _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (request.ChildVideoStreams != null)
        {
            await SynchronizeChildRelationships(videoStream, request.ChildVideoStreams, cancellationToken).ConfigureAwait(false);
        }

        _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        var ret = _mapper.Map<VideoStreamDto>(videoStream);

        if (videoStream.ChildVideoStreams.Any())
        {
            var ids = videoStream.ChildVideoStreams.Select(a => a.ChildVideoStreamId).ToList();
            var streams = RepositoryContext.VideoStreams.Where(a => ids.Contains(a.Id)).ToList();
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
    private async Task RemoveNonExistingVideoStreamLinksAsync(string parentVideoStreamId, List<ChildVideoStreamDto> existingVideoStreamLinks, CancellationToken cancellationToken)
    {
        var existingLinkIds = existingVideoStreamLinks.Select(vsl => vsl.Id).ToList();

        var linksToRemove = await RepositoryContext.VideoStreamLinks
            .Where(vsl => !existingLinkIds.Contains(vsl.ChildVideoStreamId) && vsl.ParentVideoStreamId == parentVideoStreamId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        RepositoryContext.VideoStreamLinks.RemoveRange(linksToRemove);

        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    }

    private async Task AddOrUpdateChildToVideoStreamAsync(string parentVideoStreamId, string childId, int rank, CancellationToken cancellationToken)
    {
        var videoStreamLink = await RepositoryContext.VideoStreamLinks
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
    public async Task<IEnumerable<VideoStream>> GetAllVideoStreamsAsync()
    {
        return await FindAll()
                        .OrderBy(p => p.Id)
                        .ToListAsync();
    }
    public async Task<VideoStreamDto> GetVideoStreamDtoByIdAsync(string Id, CancellationToken cancellationToken = default)
    {
        var stream = await GetVideoStreamByIdAsync(Id, cancellationToken).ConfigureAwait(false);

        VideoStreamDto videoStreamDto = _mapper.Map<VideoStreamDto>(stream);

        return videoStreamDto;
    }
    public async Task<VideoStream> GetVideoStreamByIdAsync(string Id, CancellationToken cancellationToken = default)
    {
        var stream = await GetVideoStreamWithChildrenAsync(Id, cancellationToken).ConfigureAwait(false);

        return stream;
    }

    private IQueryable<VideoStream> GetAllVideoStreamsWithChildren()
    {
        return RepositoryContext.VideoStreams
            .Include(vs => vs.ChildVideoStreams)
            .ThenInclude(vsl => vsl.ChildVideoStream);
    }

    public IQueryable<VideoStream> GetVideoStreamsChannelGroupName(string channelGroupName)
    {
        var streams = GetAllVideoStreamsWithChildren();
        var ret = streams.Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroupName.ToLower());

        return ret;
    }

    public IQueryable<VideoStream> GetVideoStreamsByM3UFileId(int m3uFileId)
    {
        var streams = GetAllVideoStreamsWithChildren();
        var ret = streams.Where(a => a.M3UFileId == m3uFileId);

        return ret;
    }
    public IQueryable<VideoStream> GetVideoStreamsByMatchingIds(IEnumerable<string> ids)
    {
        var streams = GetAllVideoStreamsWithChildren();
        var ret = streams.Where(a => ids.Contains(a.Id));

        return ret;
    }

    public IQueryable<VideoStream> GetVideoStreamsHidden()
    {
        var streams = GetAllVideoStreamsWithChildren();
        var ret = streams.Where(a => !a.IsHidden);

        return ret;
    }
    public async Task<PagedList<VideoStream>> GetVideoStreamsAsync(VideoStreamParameters VideoStreamParameters, CancellationToken cancellationToken)
    {
        var streams = GetAllVideoStreamsWithChildren();
        var sorderVideoStreams = _VideoStreamSortHelper.ApplySort(streams, VideoStreamParameters.OrderBy);

        return await PagedList<VideoStream>.ToPagedList(sorderVideoStreams, VideoStreamParameters.PageNumber, VideoStreamParameters.PageSize);
    }

    public void UpdateVideoStream(VideoStream VideoStream)
    {
        Update(VideoStream);
    }
}
