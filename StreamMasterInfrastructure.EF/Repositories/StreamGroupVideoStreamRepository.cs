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

    public async Task<List<VideoStreamDto>> GetStreamGroupVideoStreams(int id, CancellationToken cancellationToken = default)
    {
        if (id == 0)
        {
            return new();
        }

        StreamGroup? streamGroup = await _repository.StreamGroup.GetStreamGroupWithRelatedEntitiesByIdAsync(id, cancellationToken);

        if (streamGroup == null)
        {
            return new();
        }

        List<VideoStream> cvs = streamGroup.ChildVideoStreams.Select(a => a.ChildVideoStream).ToList();
        List<VideoStreamDto> ret = _mapper.Map<List<VideoStreamDto>>(cvs);

        List<string> existingIds = ret.Select(a => a.Id).ToList();

        List<string> cgNames = streamGroup.ChannelGroups.Select(a => a.ChannelGroup.Name).ToList();

        var streams = await RepositoryContext.VideoStreams
                .Where(a => !existingIds.Contains(a.Id) && cgNames.Contains(a.User_Tvg_group))
                .Select(a => a.Id)
                .AsNoTracking().ToListAsync();

        if (streams.Any())
        {
            List<VideoStreamDto> streamsDto = _mapper.Map<List<VideoStreamDto>>(streams);
            ret.AddRange(streamsDto);
        }

        var links = await RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == id).ToListAsync(cancellationToken: cancellationToken);

        foreach (var stream in ret)
        {
            stream.Rank = links.Single(a => a.ChildVideoStreamId == stream.Id).Rank;
        }

        return ret;
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