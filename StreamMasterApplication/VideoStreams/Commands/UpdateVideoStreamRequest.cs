using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.VideoStreams.Events;

using System.Diagnostics;

namespace StreamMasterApplication.VideoStreams.Commands;

public class UpdateVideoStreamRequestValidator : AbstractValidator<UpdateVideoStreamRequest>
{
    public UpdateVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().NotEmpty();
    }
}

public class UpdateVideoStreamRequestHandler : BaseMemoryRequestHandler, IRequestHandler<UpdateVideoStreamRequest, bool>
{

    public UpdateVideoStreamRequestHandler(ILogger<UpdateVideoStreamRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache)
    { }

    public async Task<bool> Handle(UpdateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        Stopwatch stopWatch = Stopwatch.StartNew();

        bool ret = await Repository.VideoStream.UpdateVideoStreamAsync(request, cancellationToken).ConfigureAwait(false);

        if (ret)
        {
            if (request.IsHidden != null)
            {
                await Publisher.Publish(new UpdateChannelGroupEvent(), cancellationToken).ConfigureAwait(false);
            }
            await Publisher.Publish(new UpdateVideoStreamEvent(), cancellationToken).ConfigureAwait(false);
        }
        stopWatch.Stop();
        Logger.LogInformation($"UpdateVideoStreamRequestHandler - ElapsedMilliseconds: {stopWatch.ElapsedMilliseconds}");
        return ret;
    }
}
