﻿using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructureEF.Repositories;

public class VideoStreamLinkRepository(ILogger<VideoStreamLinkRepository> logger, RepositoryContext repositoryContext, IMapper mapper, IMemoryCache memoryCache, ISender sender) : RepositoryBase<VideoStreamLink>(repositoryContext, logger), IVideoStreamLinkRepository
{
    public async Task<List<string>> GetVideoStreamVideoStreamIds(string videoStreamId, CancellationToken cancellationToken)
    {
        List<string> ids = await FindByCondition(a => a.ParentVideoStreamId == videoStreamId).OrderBy(a => a.Rank).Select(a => a.ChildVideoStreamId).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return ids;
    }
    public async Task<PagedResponse<ChildVideoStreamDto>> GetVideoStreamVideoStreams(VideoStreamLinkParameters parameters, CancellationToken cancellationToken)
    {
        parameters.OrderBy = "rank";

        IIncludableQueryable<VideoStreamLink, VideoStream> entities = GetIQueryableForEntity(parameters).Include(a => a.ChildVideoStream);

        IPagedList<VideoStreamLink> pagedResult = await entities.ToPagedListAsync(parameters.PageNumber, parameters.PageSize).ConfigureAwait(false);

        // If there are no entities, return an empty response early
        if (!pagedResult.Any())
        {
            return new PagedResponse<ChildVideoStreamDto>
            {
                PageNumber = parameters.PageNumber,
                TotalPageCount = 0,
                PageSize = parameters.PageSize,
                TotalItemCount = 0,
                Data = new List<ChildVideoStreamDto>()
            };
        }

        //var ids = links.Select(a => a.ChildVideoStreamId);

        //var videoStreams = await RepositoryContext.VideoStreams.Where(a => ids.Contains(a.Id)).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        List<ChildVideoStreamDto> cgs = new();

        //var links = await FindByCondition(a => a.ParentVideoStreamId == videoStreamId).ToArrayAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        foreach (VideoStreamLink? link in pagedResult)
        {
            ChildVideoStreamDto cg = mapper.Map<ChildVideoStreamDto>(link.ChildVideoStream);
            cg.Rank = link.Rank;
            cgs.Add(cg);
        }

        StaticPagedList<ChildVideoStreamDto> test = new(cgs, pagedResult.GetMetaData());

        PagedResponse<ChildVideoStreamDto> pagedResponse = test.ToPagedResponse();

        return pagedResponse;
    }

    public VideoStreamLink GetVideoStreamLink(string ParentVideoStreamId, string ChildVideoStreamId, int? Rank)
    {
        Rank ??= Count();
        return new VideoStreamLink { ParentVideoStreamId = ParentVideoStreamId, ChildVideoStreamId = ChildVideoStreamId, Rank = (int)Rank };
    }

    public async Task AddVideoStreamTodVideoStream(string ParentVideoStreamId, string ChildVideoStreamId, int? Rank, CancellationToken cancellationToken)
    {
        List<VideoStreamLink> childVideoStreamIds = await FindByCondition(a => a.ParentVideoStreamId == ParentVideoStreamId).OrderBy(a => a.Rank).AsNoTracking().ToListAsync(cancellationToken: cancellationToken);

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

        VideoStreamLink newL = GetVideoStreamLink(ParentVideoStreamId, ChildVideoStreamId, Rank = rank);
        Create(newL);
        await RepositoryContext.SaveChangesAsync(cancellationToken);

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
        VideoStreamLink exists = FindByCondition(a => a.ParentVideoStreamId == ParentVideoStreamId && a.ChildVideoStreamId == ChildVideoStreamId).Single();
        if (exists != null)
        {
            Delete(exists);
            await RepositoryContext.SaveChangesAsync(cancellationToken);
        }
    }
}