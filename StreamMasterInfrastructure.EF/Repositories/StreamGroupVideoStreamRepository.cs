using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;


namespace StreamMasterInfrastructureEF.Repositories;

public class StreamGroupVideoStreamRepository(RepositoryContext repositoryContext, IRepositoryWrapper repository, IMapper mapper, ISender sender) : RepositoryBase<StreamGroupVideoStream>(repositoryContext), IStreamGroupVideoStreamRepository
{
    private readonly IMapper _mapper = mapper;
    private readonly ISender _sender = sender;
    private readonly IRepositoryWrapper _repository = repository;

    public async Task AddStreamGroupVideoStreams(int StreamGroupId, List<string> toAdd, bool IsReadOnly, CancellationToken cancellationToken)
    {
        try
        {
            StreamGroup? streamGroup = await _repository.StreamGroup.GetStreamGroupByIdAsync(StreamGroupId).ConfigureAwait(false);

            if (streamGroup == null)
            {
                return;
            }

            List<string> existing = await FindByCondition(a => a.StreamGroupId == StreamGroupId).Select(a => a.ChildVideoStreamId).ToListAsync(cancellationToken).ConfigureAwait(false);

            List<string> toRun = toAdd.Except(existing).ToList();
            if (!toRun.Any())
            {
                return;
            }

            List<StreamGroupVideoStream> streamGroupVideoStreams = toRun.Select(videoStreamId => new StreamGroupVideoStream
            {
                StreamGroupId = StreamGroupId,
                ChildVideoStreamId = videoStreamId,
                IsReadOnly = IsReadOnly
            }).ToList();

            BulkInsert(streamGroupVideoStreams);

            await UpdateRanks(StreamGroupId, cancellationToken);

        }
        catch (Exception)
        {
        }
    }


    public async Task<PagedResponse<VideoStreamDto>> GetStreamGroupVideoStreams(StreamGroupVideoStreamParameters Parameters, CancellationToken cancellationToken = default)
    {
        if (Parameters == null || string.IsNullOrEmpty(Parameters.JSONFiltersString) || Parameters.JSONFiltersString == @"[]")
        {
            return new PagedResponse<VideoStreamDto>
            {
                PageNumber = 0,
                TotalPageCount = 0,
                PageSize = 0,
                TotalItemCount = 0,
                Data = new List<VideoStreamDto>()
            };
        }
        Parameters.OrderBy = "rank";
        IIncludableQueryable<StreamGroupVideoStream, VideoStream> test = GetIQueryableForEntity(Parameters)
                       .Include(sg => sg.ChildVideoStream);
        string testq = test.ToQueryString();
        List<StreamGroupVideoStream>? streamGroup = await GetIQueryableForEntity(Parameters)
                       .Include(sg => sg.ChildVideoStream)
                       .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        List<VideoStream> cvs = streamGroup.Select(a => a.ChildVideoStream).ToList();

        if (streamGroup is null || !streamGroup.Any() || !cvs.Any())
        {
            return new PagedResponse<VideoStreamDto>
            {
                PageNumber = Parameters.PageNumber,
                TotalPageCount = 0,
                PageSize = Parameters.PageSize,
                TotalItemCount = 0,
                Data = new List<VideoStreamDto>()
            };
        }

        IEnumerable<VideoStreamDto> videoStreams = _mapper.Map<IEnumerable<VideoStreamDto>>(cvs);
        IPagedList<VideoStreamDto> pagedResult = await videoStreams.ToPagedListAsync(Parameters.PageNumber, Parameters.PageSize).ConfigureAwait(false);
        PagedResponse<VideoStreamDto> pagedResponse = pagedResult.ToPagedResponse(pagedResult.TotalItemCount);

        return pagedResponse;
    }
    public async Task AddVideoStreamToStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken)
    {
        try
        {
            StreamGroup? streamGroup = await _repository.StreamGroup.GetStreamGroupByIdAsync(StreamGroupId).ConfigureAwait(false);

            if (streamGroup == null)
            {
                return;
            }

            if (FindByCondition(a => a.StreamGroupId == StreamGroupId && a.ChildVideoStreamId == VideoStreamId).Any())
            {
                return;
            }


            Create(new StreamGroupVideoStream { StreamGroupId = StreamGroupId, ChildVideoStreamId = VideoStreamId, Rank = Count() });
            await UpdateRanks(StreamGroupId, cancellationToken);

        }
        catch (Exception)
        {
        }
    }
    public async Task<List<VideoStreamIsReadOnly>> GetStreamGroupVideoStreamIds(int id, CancellationToken cancellationToken = default)
    {
        if (id == 0)
        {
            return new();
        }

        List<VideoStreamIsReadOnly> ret = await RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == id)
            .AsNoTracking()
            .Select(a => a.ChildVideoStreamId)
            .Select(a => new VideoStreamIsReadOnly { VideoStreamId = a, IsReadOnly = false }).ToListAsync(cancellationToken: cancellationToken);

        return ret.OrderBy(a => a.Rank).ToList();
    }



    public async Task RemoveStreamGroupVideoStreams(int StreamGroupId, List<string> toRemove, CancellationToken cancellationToken)
    {
        IQueryable<StreamGroupVideoStream> SGtoRemove = FindByCondition(a => a.StreamGroupId == StreamGroupId && toRemove.Contains(a.ChildVideoStreamId));
        await RemoveVideoStreamFromStreamGroup(StreamGroupId, SGtoRemove, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveVideoStreamFromStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken = default)
    {
        IQueryable<StreamGroupVideoStream> toRemove = FindByCondition(a => a.StreamGroupId == StreamGroupId && a.ChildVideoStreamId == VideoStreamId);
        await RemoveVideoStreamFromStreamGroup(StreamGroupId, toRemove, cancellationToken).ConfigureAwait(false);
    }

    public async Task SetVideoStreamRanks(int StreamGroupId, List<VideoStreamIDRank> videoStreamIDRanks, CancellationToken cancellationToken)
    {
        StreamGroup? streamGroup = await _repository.StreamGroup.GetStreamGroupByIdAsync(StreamGroupId).ConfigureAwait(false);

        if (streamGroup == null)
        {
            return;
        }

        List<StreamGroupVideoStream> existing = FindByCondition(a => a.StreamGroupId == StreamGroupId).ToList();
        foreach (VideoStreamIDRank videoStreamIDRank in videoStreamIDRanks)
        {
            StreamGroupVideoStream? streamGroupVideoStream = existing.FirstOrDefault(a => a.ChildVideoStreamId == videoStreamIDRank.VideoStreamId);
            if (streamGroupVideoStream != null && streamGroupVideoStream.Rank != videoStreamIDRank.Rank)
            {
                streamGroupVideoStream.Rank = videoStreamIDRank.Rank;
                Update(streamGroupVideoStream);
            }
        }
        await _repository.SaveAsync().ConfigureAwait(false);
    }

    private async Task RemoveVideoStreamFromStreamGroup(int StreamGroupId, IQueryable<StreamGroupVideoStream> toRemove, CancellationToken cancellationToken = default)
    {
        try
        {
            StreamGroup? streamGroup = await _repository.StreamGroup.GetStreamGroupByIdAsync(StreamGroupId).ConfigureAwait(false);

            if (streamGroup == null)
            {
                return;
            }

            BulkDelete(toRemove);
            await _repository.SaveAsync().ConfigureAwait(false);

            await UpdateRanks(StreamGroupId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception)
        {
        }
    }

    private async Task UpdateRanks(int StreamGroupId, CancellationToken cancellationToken)
    {
        List<StreamGroupVideoStream> sgVs = await FindByCondition(a => a.StreamGroupId == StreamGroupId).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        for (int i = 0; i < sgVs.Count; i++)
        {
            sgVs[i].Rank = i;
            Update(sgVs[i]);
        }

        await _repository.SaveAsync().ConfigureAwait(false);
    }
}