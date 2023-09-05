using AutoMapper;

using EFCore.BulkExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.M3UFiles.Queries;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Enums;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructureEF.Repositories;

public class VideoStreamRepository : RepositoryBase<VideoStream>, IVideoStreamRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public VideoStreamRepository(RepositoryContext repositoryContext, IMapper mapper, IMemoryCache memoryCache, ISender sender) : base(repositoryContext)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
        _sender = sender;
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

    private void CreateVideoStream(VideoStream VideoStream)
    {
        Create(VideoStream);
    }

    private void DeleteVideoStream(VideoStream VideoStream)
    {
        Delete(VideoStream);
    }

    public async Task<int> SetGroupNameByGroupName(string channelGroupName, string newGroupName, CancellationToken cancellationToken)
    {
        return await RepositoryContext.VideoStreams
              .Where(a => a.User_Tvg_group != null && a.User_Tvg_group == channelGroupName)
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

        }

        // Remove associated VideoStreamLinks where the VideoStream is a child
        List<VideoStreamLink> childLinks = await RepositoryContext.VideoStreamLinks
            .Where(vsl => videoStreamIds.Contains(vsl.ChildVideoStreamId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        if (childLinks.Count > 0)
        {
            RepositoryContext.VideoStreamLinks.RemoveRange(childLinks);

        }

        List<StreamGroupVideoStream> streamgroupLinks = await RepositoryContext.StreamGroupVideoStreams
          .Where(vsl => videoStreamIds.Contains(vsl.ChildVideoStreamId))
          .ToListAsync(cancellationToken)
          .ConfigureAwait(false);
        if (streamgroupLinks.Count > 0)
        {
            RepositoryContext.StreamGroupVideoStreams.RemoveRange(streamgroupLinks);

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
            await UpdateChannelGroupCountsFromStream(videoStream, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<int> SetGroupVisibleByGroupName(string channelGroupName, bool isHidden, CancellationToken cancellationToken)
    {
        return await RepositoryContext.VideoStreams
                   .Where(a => a.User_Tvg_group != null && a.User_Tvg_group == channelGroupName)
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
        //Setting setting = FileUtil.GetSetting();
        //string group = string.IsNullOrEmpty(request.Tvg_group) ? "(None)" : request.Tvg_group;
        //string epgId = string.IsNullOrEmpty(request.Tvg_ID) ? "" : request.Tvg_ID;

        VideoStream videoStream = new()
        {
            Id = IdConverter.GetID(),
            IsUserCreated = true,
            M3UFileName = "CUSTOM",

            //Tvg_chno = request.Tvg_chno is null ? 0 : (int)request.Tvg_chno,
            //User_Tvg_chno = request.Tvg_chno is null ? 0 : (int)request.Tvg_chno,

            //Tvg_group = group,
            //User_Tvg_group = group,

            //Tvg_ID = epgId,
            //User_Tvg_ID = epgId,

            //Tvg_logo = request.Tvg_logo is null ? setting.StreamMasterIcon : request.Tvg_logo,
            //User_Tvg_logo = request.Tvg_logo is null ? setting.StreamMasterIcon : request.Tvg_logo,

            //Tvg_name = request.Tvg_name,
            //User_Tvg_name = request.Tvg_name,

            //Url = request.Url ?? string.Empty,
            //User_Url = request.Url ?? string.Empty
        };

        videoStream = await UpdateVideoStreamValues(videoStream, request, cancellationToken).ConfigureAwait(false);
        CreateVideoStream(videoStream);

        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (request.ChildVideoStreams != null)
        {
            await SynchronizeChildRelationships(videoStream, request.ChildVideoStreams, cancellationToken).ConfigureAwait(false);
        }

        await UpdateChannelGroupCountsFromStream(videoStream, cancellationToken).ConfigureAwait(false);

        return videoStream;
    }

    private async Task UpdateChannelGroupCountsFromStream(VideoStream videoStream, CancellationToken cancellationToken)
    {
        //VideoStreamDto dto = _mapper.Map<VideoStreamDto>(videoStream);
        //List<string> channelGroupNames = await _sender.Send(new GetChannelGroupIdsFromVideoStream(dto), cancellationToken).ConfigureAwait(false);
        await _sender.Send(new UpdateChannelGroupCountRequest(videoStream.User_Tvg_group), cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> UpdateVideoStreamAsync(UpdateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        VideoStream? videoStream = await GetVideoStreamWithChildrenAsync(request.Id, cancellationToken).ConfigureAwait(false);
        if (videoStream == null)
        {
            return false;
        }

        videoStream = await UpdateVideoStreamValues(videoStream, request, cancellationToken).ConfigureAwait(false);
        UpdateVideoStream(videoStream);

        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (request.ChildVideoStreams != null)
        {
            await SynchronizeChildRelationships(videoStream, request.ChildVideoStreams, cancellationToken).ConfigureAwait(false);
        }

        await UpdateChannelGroupCountsFromStream(videoStream, cancellationToken).ConfigureAwait(false);

        return true;
    }

    private Task<VideoStream> UpdateVideoStreamValues(VideoStream videoStream, VideoStreamBaseRequest request, CancellationToken cancellationToken)
    {
        Setting setting = FileUtil.GetSetting();

        MergeVideoStream(videoStream, request);
        bool epglogo = false;

        if (request.Tvg_name != null && (videoStream.User_Tvg_name != request.Tvg_name || videoStream.IsUserCreated))
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

        if (request.Tvg_ID != null && (videoStream.User_Tvg_ID != request.Tvg_ID || videoStream.IsUserCreated))
        {
            //string? test = _memoryCache.GetEPGChannelNameByDisplayName(request.Tvg_ID);
            videoStream.User_Tvg_ID = request.Tvg_ID;
            if (setting.VideoStreamAlwaysUseEPGLogo && videoStream.User_Tvg_ID != null)
            {
                string? logoUrl = _memoryCache.GetEPGChannelLogoByTvgId(videoStream.User_Tvg_ID);
                if (logoUrl != null)
                {
                    videoStream.User_Tvg_logo = logoUrl;
                    epglogo = true;
                }
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
                List<IconFileDto> icons = _memoryCache.GetIcons(_mapper);
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
        else if (request.IsHidden != null && (videoStream.IsHidden != request.IsHidden || videoStream.IsUserCreated))
        {
            videoStream.IsHidden = request.IsHidden.Value;
        }

        return Task.FromResult(videoStream);
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

    public IQueryable<VideoStream> GetJustVideoStreams()
    {
        return FindAll();
    }

    public IQueryable<VideoStream> GetAllVideoStreams()
    {
        return FindAll()
             .Include(vs => vs.ChildVideoStreams)
            .ThenInclude(vsl => vsl.ChildVideoStream)
                        .OrderBy(p => p.Id);
    }

    public IQueryable<string> GetVideoStreamNames()
    {
        return FindAll().Select(a => a.User_Tvg_name).Distinct();
    }

    public async Task<VideoStreamDto> GetVideoStreamDtoByIdAsync(string Id, CancellationToken cancellationToken = default)
    {
        VideoStream? stream = await GetVideoStreamByIdAsync(Id, cancellationToken).ConfigureAwait(false);

        if (stream == null)
        {
            return null;
        }

        VideoStreamDto videoStreamDto = _mapper.Map<VideoStreamDto>(stream);

        return videoStreamDto;
    }

    public async Task<VideoStream?> GetVideoStreamByIdAsync(string Id, CancellationToken cancellationToken = default)
    {
        VideoStream? stream = await GetVideoStreamWithChildrenAsync(Id, cancellationToken).ConfigureAwait(false);

        return stream;
    }

    //private static List<string> CleanUpFilterValue(string? value)
    //{
    //    if (value == null)
    //    {
    //        return new();
    //    }

    //    return Regex.Matches(value, "\"(.*?)\"")
    //                .Cast<Match>()
    //                .Select(m => m.Groups[1].Value)
    //                .ToList();
    //}

    public async Task<PagedResponse<VideoStreamDto>> GetVideoStreams(VideoStreamParameters VideoStreamParameters, CancellationToken cancellationToken)
    {
        PagedResponse<VideoStreamDto> filteredStreams = await GetVideoStreamsAsync(VideoStreamParameters, cancellationToken);
        return filteredStreams;
    }

    public IQueryable<VideoStream> GetVideoStreamsByM3UFileId(int m3uFileId)
    {
        IQueryable<VideoStream> streams = GetAllVideoStreams();
        IQueryable<VideoStream> ret = streams.Where(a => a.M3UFileId == m3uFileId);

        return ret;
    }

    public IQueryable<VideoStream> GetVideoStreamsById(string id)
    {
        IQueryable<VideoStream> streams = GetAllVideoStreams();
        IQueryable<VideoStream> ret = streams.Where(a => a.Id == id);
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

    private async Task<PagedResponse<VideoStreamDto>> GetVideoStreamsAsync(VideoStreamParameters videoStreamParameters, CancellationToken cancellationToken)
    {
        return await GetEntitiesAsync<VideoStreamDto>(videoStreamParameters, _mapper);
    }

    private void UpdateVideoStream(VideoStream VideoStream)
    {
        Update(VideoStream);
    }
    public async Task<bool> DeleteAllVideoStreamsFromParameters(VideoStreamParameters Parameters, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> toDelete = GetIQueryableForEntity(Parameters).Where(a => a.IsUserCreated);
        //List<VideoStream> test = toDelete.ToList();
        await RepositoryContext.BulkDeleteAsync(toDelete, cancellationToken: cancellationToken).ConfigureAwait(false);
        //await RepositoryContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateAllVideoStreamsFromParameters(VideoStreamParameters Parameters, UpdateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        const int batchSize = 1000;

        List<string> Ids = GetIQueryableForEntity(Parameters).Select(a => a.Id).ToList();
        for (int i = 0; i < Ids.Count; i += batchSize)
        {
            IEnumerable<string> batch = Ids.Skip(i).Take(batchSize);

            await RepositoryContext.VideoStreams
                    .Where(a => batch.Contains(a.Id))
                    .ForEachAsync(s => s.IsHidden = !s.IsHidden, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            await RepositoryContext.SaveChangesAsync(cancellationToken);
        }

        await RepositoryContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task AddVideoStreamTodVideoStream(string ParentVideoStreamId, string ChildVideoStreamId, int? Rank, CancellationToken cancellationToken)
    {
        List<VideoStreamLink> childVideoStreamIds = await RepositoryContext.VideoStreamLinks.Where(a => a.ParentVideoStreamId == ParentVideoStreamId).OrderBy(a => a.Rank).AsNoTracking().ToListAsync(cancellationToken: cancellationToken);

        childVideoStreamIds ??= new();

        if (childVideoStreamIds.Any(a => a.ChildVideoStreamId == ChildVideoStreamId))
        {
            return;
        }

        int rank = childVideoStreamIds.Count;
        if (Rank.HasValue && Rank.Value > 0 && Rank.Value < childVideoStreamIds.Count)
        {
            rank = Rank.Value;
        }

        VideoStreamLink newL = new() { ParentVideoStreamId = ParentVideoStreamId, ChildVideoStreamId = ChildVideoStreamId, Rank = rank };
        await RepositoryContext.VideoStreamLinks.AddAsync(newL, cancellationToken).ConfigureAwait(false);
        childVideoStreamIds.Insert(rank, newL);

        for (int i = 0; i < childVideoStreamIds.Count; i++)
        {
            VideoStreamLink? childVideoStreamId = childVideoStreamIds[i];
            childVideoStreamId.Rank = i;
            RepositoryContext.VideoStreamLinks.Update(childVideoStreamId);
        }

        await RepositoryContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveVideoStreamFromVideoStream(string ParentVideoStreamId, string ChildVideoStreamId, CancellationToken cancellationToken)
    {
        VideoStreamLink? exists = await RepositoryContext.VideoStreamLinks.FirstOrDefaultAsync(a => a.ParentVideoStreamId == ParentVideoStreamId && a.ChildVideoStreamId == ChildVideoStreamId, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (exists != null)
        {
            RepositoryContext.VideoStreamLinks.Remove(exists);
            await RepositoryContext.SaveChangesAsync(cancellationToken);
        }
    }



    public async Task SetVideoStreamChannelNumbersFromIds(List<string> Ids, bool OverWriteExisting, int StartNumber, string OrderBy, CancellationToken cancellationToken)
    {
        List<VideoStream> videoStreams = await FindByCondition(a => Ids.Contains(a.Id), OrderBy).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        await SetVideoStreamChannelNumbers(videoStreams, OverWriteExisting, StartNumber, cancellationToken).ConfigureAwait(false);
    }
    public async Task SetVideoStreamChannelNumbersFromParameters(VideoStreamParameters Parameters, bool OverWriteExisting, int StartNumber, CancellationToken cancellationToken)
    {
        List<VideoStream> videoStreams = await GetIQueryableForEntity(Parameters).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        await SetVideoStreamChannelNumbers(videoStreams, OverWriteExisting, StartNumber, cancellationToken).ConfigureAwait(false);
    }

    private int GetNextNumber(int startNumber, HashSet<int> existingNumbers)
    {
        while (existingNumbers.Contains(startNumber))
        {
            startNumber++;
        }
        return startNumber;
    }

    private async Task SetVideoStreamChannelNumbers(List<VideoStream> videoStreams, bool overWriteExisting, int startNumber, CancellationToken cancellationToken)
    {
        HashSet<int> existingNumbers = new();

        if (!overWriteExisting)
        {
            existingNumbers.UnionWith(videoStreams.Select(a => a.User_Tvg_chno).Distinct());
        }

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
                existingNumbers.Add(number);
            }

            UpdateVideoStream(videoStream);
        }

        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<int> SetVideoStreamsLogoFromEPGFromIds(List<string> Ids, CancellationToken cancellationToken)
    {

        List<VideoStream> videoStreams = await FindByCondition(a => Ids.Contains(a.Id)).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return await SetVideoStreamsLogoFromEPG(videoStreams, cancellationToken).ConfigureAwait(false);
    }

    public async Task<int> SetVideoStreamsLogoFromEPGFromParameters(VideoStreamParameters Parameters, CancellationToken cancellationToken)
    {
        List<VideoStream> videoStreams = await GetIQueryableForEntity(Parameters).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return await SetVideoStreamsLogoFromEPG(videoStreams, cancellationToken).ConfigureAwait(false);
    }
    private async Task<int> SetVideoStreamsLogoFromEPG(List<VideoStream> videoStreams, CancellationToken cancellationToken)
    {
        int ret = 0;
        foreach (VideoStream videoStream in videoStreams)
        {
            string? channelLogo = _memoryCache.GetEPGChannelLogoByTvgId(videoStream.User_Tvg_ID);

            if (channelLogo != null)
            {
                videoStream.User_Tvg_logo = channelLogo;
                Update(videoStream);
                ret++;
            }
        }

        if (ret > 0)
        {
            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        return ret;
    }

    public async Task<int> ReSetVideoStreamsLogoFromIds(List<string> Ids, CancellationToken cancellationToken)
    {
        List<VideoStream> videoStreams = await FindByCondition(a => Ids.Contains(a.Id)).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return await SetVideoStreamsLogo(videoStreams, cancellationToken);
    }
    public async Task<int> ReSetVideoStreamsLogoFromParameters(VideoStreamParameters Parameters, CancellationToken cancellationToken)
    {
        List<VideoStream> videoStreams = await GetIQueryableForEntity(Parameters).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return await SetVideoStreamsLogo(videoStreams, cancellationToken);
    }

    private async Task<int> SetVideoStreamsLogo(List<VideoStream> videoStreams, CancellationToken cancellationToken)
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
            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        return ret;
    }


}