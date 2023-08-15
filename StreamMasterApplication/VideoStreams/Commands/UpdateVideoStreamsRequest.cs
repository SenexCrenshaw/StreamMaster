using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.ChannelGroups.Queries;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public record UpdateVideoStreamsRequest(IEnumerable<UpdateVideoStreamRequest> VideoStreamUpdates) : IRequest<IEnumerable<VideoStreamDto>>
{
}

public class UpdateVideoStreamsRequestValidator : AbstractValidator<UpdateVideoStreamsRequest>
{
    public UpdateVideoStreamsRequestValidator()
    {
        _ = RuleFor(v => v.VideoStreamUpdates).NotNull().NotEmpty();
    }
}

public class UpdateVideoStreamsRequestHandler : BaseMemoryRequestHandler, IRequestHandler<UpdateVideoStreamsRequest, IEnumerable<VideoStreamDto>>
{

    public UpdateVideoStreamsRequestHandler(ILogger<UpdateVideoStreamsRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }
    public async Task<IEnumerable<VideoStreamDto>> Handle(UpdateVideoStreamsRequest requests, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = new();
        List<string> channelnames = new();

        foreach (UpdateVideoStreamRequest request in requests.VideoStreamUpdates)
        {
            VideoStreamDto? ret = await Repository.VideoStream.UpdateVideoStreamAsync(request, cancellationToken).ConfigureAwait(false);
            if (ret is not null)
            {
                channelnames.AddRange(await Sender.Send(new GetChannelGroupNamesFromVideoStream(ret), cancellationToken).ConfigureAwait(false));
                results.Add(ret);
            }
        }

        if (channelnames.Any())
        {
            foreach (string channelname in channelnames)
            {
                await Sender.Send(new UpdateChannelGroupCountRequest(channelname), cancellationToken).ConfigureAwait(false);
            }
            await Publisher.Publish(new UpdateChannelGroupEvent(), cancellationToken).ConfigureAwait(false);
        }

        if (results.Any())
        {

            await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);
        }

        return results;
    }
}
