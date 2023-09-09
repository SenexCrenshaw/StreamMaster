using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using StreamMasterApplication.StreamGroups.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

using System.Linq.Dynamic.Core;

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
        // Return empty response if the parameters are not valid.
        if (Parameters == null || string.IsNullOrEmpty(Parameters.JSONFiltersString) || Parameters.JSONFiltersString == @"[]")
        {
            return CreateEmptyPagedResponse<VideoStreamDto>(Parameters);
        }


        string originalOrderBy = Parameters.OrderBy?.ToLower() ?? "";
        Parameters.OrderBy = "";

        IIncludableQueryable<StreamGroupVideoStream, VideoStream> streamGroup = GetIQueryableForEntity(Parameters).Include(sg => sg.ChildVideoStream);

        if (streamGroup is null)
        {
            return CreateEmptyPagedResponse<VideoStreamDto>(Parameters);
        }

        List<StreamGroupVideoStream> sgStreams = await streamGroup.ToListAsync(cancellationToken).ConfigureAwait(false);
        IEnumerable<VideoStream> cvs = sgStreams.Select(a => a.ChildVideoStream);

        if (!cvs.Any())
        {
            return CreateEmptyPagedResponse<VideoStreamDto>(Parameters);
        }

        if (!string.IsNullOrEmpty(originalOrderBy))
        {
            if (originalOrderBy.StartsWith("user_tvg_chno"))
            {
                cvs = originalOrderBy.EndsWith("desc")
                    ? cvs.OrderByDescending(a => a.User_Tvg_chno).ToList()
                    : cvs.OrderBy(a => a.User_Tvg_chno).ToList();
            }
            else if (originalOrderBy.StartsWith("user_tvg_name"))
            {
                cvs = originalOrderBy.EndsWith("desc")
                    ? cvs.OrderByDescending(a => a.User_Tvg_name).ToList()
                    : cvs.OrderBy(a => a.User_Tvg_name).ToList();
            }
            else if (originalOrderBy.StartsWith("user_tvg_id"))
            {
                cvs = originalOrderBy.EndsWith("desc")
                    ? cvs.OrderByDescending(a => a.User_Tvg_ID).ToList()
                    : cvs.OrderBy(a => a.User_Tvg_ID).ToList();
            }
        }

        IEnumerable<VideoStreamDto> videoStreams = _mapper.Map<IEnumerable<VideoStreamDto>>(cvs);
        IPagedList<VideoStreamDto> pagedResult = await videoStreams.ToPagedListAsync(Parameters.PageNumber, Parameters.PageSize).ConfigureAwait(false);
        PagedResponse<VideoStreamDto> pagedResponse = pagedResult.ToPagedResponse(pagedResult.TotalItemCount);

        IEnumerable<string> cnames = videoStreams.Select(a => a.User_Tvg_group).Distinct();
        List<ChannelGroup> channelGroups = RepositoryContext.ChannelGroups.Where(a => cnames.Contains(a.Name)).AsNoTracking().ToList();

        foreach (VideoStreamDto videoStream in pagedResponse.Data)
        {
            videoStream.IsReadOnly = sgStreams.FirstOrDefault(a => a.ChildVideoStreamId == videoStream.Id)?.IsReadOnly ?? false;
            videoStream.ChannelGroupId = channelGroups.FirstOrDefault(a => a.Name == videoStream.User_Tvg_group)?.Id ?? 0;
        }



        return pagedResponse;
    }
    public async Task<StreamGroupDto?> AddVideoStreamToStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken)
    {
        try
        {
            StreamGroup? streamGroup = await _repository.StreamGroup.GetStreamGroupByIdAsync(StreamGroupId).ConfigureAwait(false);

            if (streamGroup == null)
            {
                return null;
            }

            if (FindByCondition(a => a.StreamGroupId == StreamGroupId && a.ChildVideoStreamId == VideoStreamId).Any())
            {
                return null;
            }

            Create(new StreamGroupVideoStream { StreamGroupId = StreamGroupId, ChildVideoStreamId = VideoStreamId, Rank = Count() });
            await UpdateRanks(StreamGroupId, cancellationToken);
            return await _sender.Send(new GetStreamGroup(StreamGroupId), cancellationToken);
        }
        catch (Exception)
        {
        }
        return null;
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

    public async Task RemoveStreamGroupVideoStreams(int StreamGroupId, IEnumerable<string> toRemove, CancellationToken cancellationToken)
    {
        StreamGroup? streamGroup = await _repository.StreamGroup.GetStreamGroupByIdAsync(StreamGroupId).ConfigureAwait(false);

        if (streamGroup == null)
        {
            return;
        }

        IQueryable<StreamGroupVideoStream> toDelete = FindByCondition(a => a.StreamGroupId == StreamGroupId && toRemove.Contains(a.ChildVideoStreamId));
        //await RepositoryContext.BulkDeleteAsync(toDelete, cancellationToken: cancellationToken).ConfigureAwait(false);

        RepositoryContext.StreamGroupVideoStreams.RemoveRange(toDelete);
        await _repository.SaveAsync().ConfigureAwait(false);

        await UpdateRanks(StreamGroupId, cancellationToken).ConfigureAwait(false);
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

    public async Task SetStreamGroupVideoStreamsIsReadOnly(int StreamGroupId, List<string> toUpdate, bool IsReadOnly, CancellationToken cancellationToken)
    {
        await FindAll()
               .Where(a => a.StreamGroupId == StreamGroupId && toUpdate.Contains(a.ChildVideoStreamId))
               .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsReadOnly, IsReadOnly), cancellationToken: cancellationToken)
               .ConfigureAwait(false);
    }

    public async Task<StreamGroupDto?> SyncVideoStreamToStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken = default)
    {
        // Fetch the stream group by ID
        StreamGroup? targetStreamGroup = await _repository.StreamGroup.GetStreamGroupByIdAsync(StreamGroupId).ConfigureAwait(false);

        if (targetStreamGroup == null)
        {
            return null;
        }

        // Check if the stream is already associated with the group
        StreamGroupVideoStream? isStreamAssociated = _repository.StreamGroupVideoStream
            .FindByCondition(stream => stream.StreamGroupId == StreamGroupId && stream.ChildVideoStreamId == VideoStreamId)
            .FirstOrDefault();

        if (isStreamAssociated != null)
        {
            if (isStreamAssociated.IsReadOnly)
            {
                return null;
            }
            await RemoveStreamGroupVideoStreams(StreamGroupId, new List<string> { VideoStreamId }, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await AddVideoStreamToStreamGroup(StreamGroupId, VideoStreamId, cancellationToken).ConfigureAwait(false);
        }

        // Fetch and return the updated stream group
        return await _sender.Send(new GetStreamGroup(StreamGroupId), cancellationToken);
    }
}