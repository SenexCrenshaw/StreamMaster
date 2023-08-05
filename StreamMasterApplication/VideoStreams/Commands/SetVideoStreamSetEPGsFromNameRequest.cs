using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public class SetVideoStreamSetEPGsFromNameRequest : IRequest<List<VideoStreamDto>>
{
    public List<string> Ids { get; set; } = new List<string>();
}

public class SetVideoStreamSetEPGsFromNameRequestHandler : BaseDBRequestHandler, IRequestHandler<SetVideoStreamSetEPGsFromNameRequest, List<VideoStreamDto>>
{

    public SetVideoStreamSetEPGsFromNameRequestHandler(IAppDbContext context, ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, context, memoryCache) { }

    public async Task<List<VideoStreamDto>> Handle(SetVideoStreamSetEPGsFromNameRequest request, CancellationToken cancellationToken)
    {
        var results = new List<VideoStreamDto>();

        var videoStreamsRepo = await Repository.VideoStream.GetAllVideoStreamsAsync().ConfigureAwait(false);
        var videoStreams = videoStreamsRepo.Where(a => request.Ids.Contains(a.Id));

        foreach (var videoStream in videoStreams)
        {

            var test = MemoryCache.GetEPGNameTvgName(videoStream.User_Tvg_name);
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
