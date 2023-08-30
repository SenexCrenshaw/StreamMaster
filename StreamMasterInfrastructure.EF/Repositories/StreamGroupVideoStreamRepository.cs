using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

namespace StreamMasterInfrastructureEF.Repositories;

public class StreamGroupVideoStreamRepository : RepositoryBase<StreamGroup>, IStreamGroupVideoStreamRepository
{
    private readonly IMapper _mapper;
    private readonly ISender _sender;
    private readonly IRepositoryWrapper _repository;

    public StreamGroupVideoStreamRepository(RepositoryContext repositoryContext, IRepositoryWrapper repository, IMapper mapper, ISender sender) : base(repositoryContext)
    {
        _sender = sender;
        _mapper = mapper;

        _repository = repository;
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

            StreamGroupDto? streamGroupDto = await _repository.StreamGroup.GetStreamGroupDto(StreamGroupId, "", cancellationToken).ConfigureAwait(false);

            if (streamGroupDto == null)
            {
                return;
            }

            var sgVs = await RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == StreamGroupId).AsNoTracking()
                .Select(a => new VideoStreamIsReadOnly { VideoStreamId = a.ChildVideoStreamId, IsReadOnly = a.IsReadOnly, Rank = a.Rank }).ToListAsync(cancellationToken: cancellationToken);
            sgVs.Add(new VideoStreamIsReadOnly { VideoStreamId = VideoStreamId, IsReadOnly = false, Rank = sgVs.Count });

            await _repository.StreamGroup.Sync(StreamGroupId, "", null, sgVs, cancellationToken);
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
            .Select(a => new VideoStreamIsReadOnly { VideoStreamId = a, IsReadOnly = false }).ToListAsync();

        List<string> existingIds = ret.Select(a => a.VideoStreamId).ToList();

        List<string> cgNames = await RepositoryContext.StreamGroupChannelGroups.AsNoTracking()
            .Where(a => a.StreamGroupId == id)
            .Select(a => a.ChannelGroup.Name)
            .ToListAsync();

        List<VideoStreamIsReadOnly> streams = await RepositoryContext.VideoStreams
                .Where(a => !existingIds.Contains(a.Id) && cgNames.Contains(a.User_Tvg_group))
                .Select(a => a.Id)
                .AsNoTracking()
                .Select(a => new VideoStreamIsReadOnly { VideoStreamId = a, IsReadOnly = true }).ToListAsync();

        ret.AddRange(streams);

        return ret;
    }

    public async Task<PagedResponse<VideoStreamDto>> GetStreamGroupVideoStreams(StreamGroupVideoStreamParameters Parameters, CancellationToken cancellationToken = default)
    {
        if (Parameters == null || string.IsNullOrEmpty(Parameters.JSONFiltersString) || Parameters.JSONFiltersString ==@"[]")
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
        Parameters.OrderBy = "id";

        var entities = GetIQueryableForEntity(Parameters).Include(sg => sg.ChannelGroups)
                .ThenInclude(sgcg => sgcg.ChannelGroup)
            .Include(sg => sg.ChildVideoStreams)
                .ThenInclude(sgvs => sgvs.ChildVideoStream);

        // If there are no entities, return an empty response early
        if (!entities.Any())
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

        var pagedResult = await entities.ToPagedListAsync(Parameters.PageNumber, Parameters.PageSize).ConfigureAwait(false);

        List<VideoStream> cvs = entities.SelectMany(a => a.ChildVideoStreams).Select(a => a.ChildVideoStream).ToList();
        List<VideoStreamDto> ret = _mapper.Map<List<VideoStreamDto>>(cvs);

        List<string> existingIds = ret.Select(a => a.Id).ToList();

        List<string> cgNames = entities.SelectMany(a => a.ChannelGroups).Select(a => a.ChannelGroup.Name).ToList();

        var streams = await RepositoryContext.VideoStreams
                .Where(a => !existingIds.Contains(a.Id) && cgNames.Contains(a.User_Tvg_group))
                .Select(a => a.Id)
                .AsNoTracking().ToListAsync();

        if (streams.Any())
        {
            List<VideoStreamDto> streamsDto = _mapper.Map<List<VideoStreamDto>>(streams);
            ret.AddRange(streamsDto);
        }

        var sgId = entities.Select(a => a.Id).First();
        var links = await RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == sgId).ToListAsync(cancellationToken: cancellationToken);

        foreach (var stream in ret)
        {
            stream.Rank = links.Single(a => a.ChildVideoStreamId == stream.Id).Rank;
        }

        StaticPagedList<VideoStreamDto> test = new(ret, pagedResult.GetMetaData());

        PagedResponse<VideoStreamDto> pagedResponse = test.ToPagedResponse(pagedResult.TotalItemCount);

        return pagedResponse;
    }

    public async Task RemoveVideoStreamFromStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken = default)
    {
        try
        {
            StreamGroup? streamGroup = await _repository.StreamGroup.GetStreamGroupByIdAsync(StreamGroupId).ConfigureAwait(false);

            if (streamGroup == null)
            {
                return;
            }

            StreamGroupDto? streamGroupDto = await _repository.StreamGroup.GetStreamGroupDto(StreamGroupId, "", cancellationToken).ConfigureAwait(false);

            if (streamGroupDto == null)
            {
                return;
            }

            var sgVs = await RepositoryContext.StreamGroupVideoStreams
                 .Where(a => a.StreamGroupId == StreamGroupId && a.ChildVideoStreamId != VideoStreamId).AsNoTracking()
                 .Select(a => new VideoStreamIsReadOnly { VideoStreamId = a.ChildVideoStreamId, IsReadOnly = a.IsReadOnly }).ToListAsync(cancellationToken: cancellationToken);

            for (int i = 0; i < sgVs.Count; i++)
            {
                VideoStreamIsReadOnly? s = sgVs[i];
                s.Rank = i;
            }

            await _repository.StreamGroup.Sync(StreamGroupId, "", null, sgVs, cancellationToken);
        }
        catch (Exception)
        {
        }
    }
}