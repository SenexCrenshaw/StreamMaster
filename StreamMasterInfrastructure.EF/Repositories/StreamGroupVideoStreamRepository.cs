using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.StreamGroups.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;

namespace StreamMasterInfrastructureEF.Repositories;

public class StreamGroupVideoStreamRepository(ILogger<StreamGroupVideoStreamRepository> logger, RepositoryContext repositoryContext, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, ISender sender) : RepositoryBase<StreamGroupVideoStream>(repositoryContext, logger), IStreamGroupVideoStreamRepository
{
    public async Task AddStreamGroupVideoStreams(int StreamGroupId, List<string> toAdd, bool IsReadOnly, CancellationToken cancellationToken)
    {
        try
        {
            StreamGroupDto? streamGroup = await repository.StreamGroup.GetStreamGroupById(StreamGroupId).ConfigureAwait(false);

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

            List<StreamGroupVideoStream> streamGroupVideoStreams = toRun.ConvertAll(videoStreamId => new StreamGroupVideoStream
            {
                StreamGroupId = StreamGroupId,
                ChildVideoStreamId = videoStreamId,
                IsReadOnly = IsReadOnly
            });

            BulkInsert(streamGroupVideoStreams);

            await UpdateRanks(StreamGroupId, cancellationToken);

        }
        catch (Exception)
        {
        }
    }

    public async Task<PagedResponse<VideoStreamDto>> GetPagedStreamGroupVideoStreams(StreamGroupVideoStreamParameters Parameters, CancellationToken cancellationToken)
    {

        if (Parameters.StreamGroupId == 0 || Parameters == null)
        {
            return repository.VideoStream.CreateEmptyPagedResponse();
        }

        if (!string.IsNullOrEmpty(Parameters.JSONFiltersString) && Regex.IsMatch(Parameters.JSONFiltersString, "streamgroupid", RegexOptions.IgnoreCase))
        {
            Parameters.JSONFiltersString = Regex.Replace(Parameters.JSONFiltersString, "streamgroupid", "user_tvg_name", RegexOptions.IgnoreCase);
        }

        if (Regex.IsMatch(Parameters.OrderBy, "streamgroupid", RegexOptions.IgnoreCase))
        {
            Parameters.OrderBy = Regex.Replace(Parameters.OrderBy, "streamgroupid", "user_tvg_name", RegexOptions.IgnoreCase);
        }

        IQueryable<VideoStream> childQ = RepositoryContext.StreamGroupVideoStreams
        .Include(a => a.ChildVideoStream)
        .Where(a => a.StreamGroupId == Parameters.StreamGroupId)
        .Select(a => a.ChildVideoStream);

        PagedResponse<VideoStreamDto> pagedResponse = await childQ.GetPagedResponseWithFilterAsync<VideoStream, VideoStreamDto>(Parameters.JSONFiltersString, Parameters.OrderBy, Parameters.PageNumber, Parameters.PageSize, mapper).ConfigureAwait(false);

        List<string> streamIds = pagedResponse.Data.ConvertAll(a => a.Id);

        List<StreamGroupVideoStream> existinglinks = RepositoryContext.StreamGroupVideoStreams
            .Where(a => a.StreamGroupId == Parameters.StreamGroupId && streamIds.Contains(a.ChildVideoStreamId) && a.IsReadOnly).ToList();

        foreach (VideoStreamDto item in pagedResponse.Data)
        {
            if (existinglinks.Any(a => a.ChildVideoStreamId == item.Id))
            {
                item.IsReadOnly = true;
            }
        }

        return pagedResponse;
    }
    public async Task<StreamGroupDto?> AddVideoStreamToStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken)
    {
        try
        {
            StreamGroupDto? streamGroup = await repository.StreamGroup.GetStreamGroupById(StreamGroupId).ConfigureAwait(false);

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
            return await sender.Send(new GetStreamGroup(StreamGroupId), cancellationToken);
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
            return [];
        }

        List<VideoStreamIsReadOnly> ret = await RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == id)
            .AsNoTracking()
            .Select(a => a.ChildVideoStreamId)
            .Select(a => new VideoStreamIsReadOnly { VideoStreamId = a, IsReadOnly = false }).ToListAsync(cancellationToken: cancellationToken);

        return ret.OrderBy(a => a.Rank).ToList();
    }

    public async Task RemoveStreamGroupVideoStreams(int StreamGroupId, IEnumerable<string> toRemove, CancellationToken cancellationToken)
    {
        StreamGroupDto? streamGroup = await repository.StreamGroup.GetStreamGroupById(StreamGroupId).ConfigureAwait(false);

        if (streamGroup == null)
        {
            return;
        }

        IQueryable<StreamGroupVideoStream> toDelete = FindByCondition(a => a.StreamGroupId == StreamGroupId && toRemove.Contains(a.ChildVideoStreamId));
        //await RepositoryContext.BulkDeleteAsync(toDelete, cancellationToken: cancellationToken).ConfigureAwait(false);

        RepositoryContext.StreamGroupVideoStreams.RemoveRange(toDelete);
        await repository.SaveAsync().ConfigureAwait(false);

        await UpdateRanks(StreamGroupId, cancellationToken).ConfigureAwait(false);
    }

    public async Task SetVideoStreamRanks(int StreamGroupId, List<VideoStreamIDRank> videoStreamIDRanks, CancellationToken cancellationToken)
    {
        StreamGroupDto? streamGroup = await repository.StreamGroup.GetStreamGroupById(StreamGroupId).ConfigureAwait(false);

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
        await repository.SaveAsync().ConfigureAwait(false);
    }

    private async Task UpdateRanks(int StreamGroupId, CancellationToken cancellationToken)
    {
        List<StreamGroupVideoStream> sgVs = await FindByCondition(a => a.StreamGroupId == StreamGroupId).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        for (int i = 0; i < sgVs.Count; i++)
        {
            sgVs[i].Rank = i;
            Update(sgVs[i]);
        }

        await repository.SaveAsync().ConfigureAwait(false);
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
        StreamGroupDto? targetStreamGroup = await repository.StreamGroup.GetStreamGroupById(StreamGroupId).ConfigureAwait(false);

        if (targetStreamGroup == null)
        {
            return null;
        }

        // Check if the stream is already associated with the group
        StreamGroupVideoStream? isStreamAssociated = FindByCondition(stream => stream.StreamGroupId == StreamGroupId && stream.ChildVideoStreamId == VideoStreamId)
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
        return await sender.Send(new GetStreamGroup(StreamGroupId), cancellationToken);
    }

    public async Task<List<VideoStreamDto>> GetStreamGroupVideoStreams(int StreamGroupId, CancellationToken cancellationToken)
    {
        if (StreamGroupId < 2)
        {
            return await repository.VideoStream.GetVideoStreamsNotHidden().ConfigureAwait(false);
        }

        IQueryable<VideoStream> childQ = FindAll()
       .Include(a => a.ChildVideoStream)
       .Where(a => a.StreamGroupId == StreamGroupId)
       .Select(a => a.ChildVideoStream)
       .Where(a => !a.IsHidden);

        string test = childQ.ToQueryString();

        return await childQ.ProjectTo<VideoStreamDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);


    }
}