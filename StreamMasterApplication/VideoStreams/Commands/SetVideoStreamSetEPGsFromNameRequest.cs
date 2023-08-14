using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Events;
using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public class SetVideoStreamSetEPGsFromNameRequest : IRequest<List<VideoStreamDto>>
{
    public List<string> Ids { get; set; } = new List<string>();
}

public class SetVideoStreamSetEPGsFromNameRequestHandler : BaseMemoryRequestHandler, IRequestHandler<SetVideoStreamSetEPGsFromNameRequest, List<VideoStreamDto>>
{

    public SetVideoStreamSetEPGsFromNameRequestHandler(ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<List<VideoStreamDto>> Handle(SetVideoStreamSetEPGsFromNameRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = new();

        IQueryable<VideoStream> videoStreamsRepo = Repository.VideoStream.GetAllVideoStreams().Where(a => request.Ids.Contains(a.Id));
        IQueryable<VideoStream> videoStreams = videoStreamsRepo.Where(a => request.Ids.Contains(a.Id));

        foreach (VideoStream? videoStream in videoStreams)
        {

            string? test = MemoryCache.GetEPGNameTvgName(videoStream.User_Tvg_name);
            if (test is not null)
            {
                videoStream.User_Tvg_ID = test;
                Repository.VideoStream.Update(videoStream);
                results.Add(Mapper.Map<VideoStreamDto>(videoStream));
            }
        }

        await Repository.SaveAsync().ConfigureAwait(false);
        if (results.Count > 0)
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }

        return results;
    }
}
