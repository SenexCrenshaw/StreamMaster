using AutoMapper;

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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace StreamMasterInfrastructureEF.Repositories;

public class VideoStreamLinkRepository : RepositoryBase<VideoStreamLink>, IVideoStreamLinkRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public VideoStreamLinkRepository(RepositoryContext repositoryContext, IMapper mapper, IMemoryCache memoryCache, ISender sender) : base(repositoryContext)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
        _sender = sender;
    }

    public async Task<List<string>> GetVideoStreamVideoStreamIds(string videoStreamId, CancellationToken cancellationToken)
    {
        var ids = await FindByCondition(a => a.ParentVideoStreamId == videoStreamId).OrderBy(a => a.Rank).Select(a => a.ChildVideoStreamId).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return ids;
    }

    public async Task<List<ChildVideoStreamDto>> GetVideoStreamVideoStreams(string videoStreamId, CancellationToken cancellationToken)
    {
        var ids = await GetVideoStreamVideoStreamIds(videoStreamId, cancellationToken).ConfigureAwait(false);

        var videoStreams = await RepositoryContext.VideoStreams.Where(a => ids.Contains(a.Id)).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        var cgs = _mapper.Map<List<ChildVideoStreamDto>>(videoStreams);

        return cgs;
    }

    public VideoStreamLink GetVideoStreamLink(string ParentVideoStreamId, string ChildVideoStreamId, int? Rank)
    {
        Rank ??= Count();
        return new VideoStreamLink { ParentVideoStreamId = ParentVideoStreamId, ChildVideoStreamId = ChildVideoStreamId, Rank = (int)Rank };
    }

    public async Task AddVideoStreamTodVideoStream(string ParentVideoStreamId, string ChildVideoStreamId, int? Rank, CancellationToken cancellationToken)
    {
        var childVideoStreamIds = await FindByCondition(a => a.ParentVideoStreamId == ParentVideoStreamId).OrderBy(a => a.Rank).AsNoTracking().ToListAsync(cancellationToken: cancellationToken);

        childVideoStreamIds ??= new();

        if (childVideoStreamIds.Any(a => a.ChildVideoStreamId == ChildVideoStreamId))
        {
            return;
        }

        var rank = childVideoStreamIds.Count;
        if (Rank.HasValue && Rank.Value > 0 && Rank.Value < childVideoStreamIds.Count)
        {
            rank = Rank.Value;
        }

        var newL = GetVideoStreamLink(ParentVideoStreamId, ChildVideoStreamId, Rank = rank);
        Create(newL);
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
        var exists = await RepositoryContext.VideoStreamLinks.FirstOrDefaultAsync(a => a.ParentVideoStreamId == ParentVideoStreamId && a.ChildVideoStreamId == ChildVideoStreamId, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (exists != null)
        {
            RepositoryContext.VideoStreamLinks.Remove(exists);
            await RepositoryContext.SaveChangesAsync(cancellationToken);
        }
    }
}