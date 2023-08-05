using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public class UpdateVideoStreamRequest : VideoStreamUpdate, IRequest<VideoStreamDto?>
{
}

public class UpdateVideoStreamRequestValidator : AbstractValidator<UpdateVideoStreamRequest>
{
    public UpdateVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().NotEmpty();
    }
}

public class UpdateVideoStreamRequestHandler : BaseMediatorRequestHandler, IRequestHandler<UpdateVideoStreamRequest, VideoStreamDto?>
{

    public UpdateVideoStreamRequestHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<VideoStreamDto?> Handle(UpdateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        var ret = await Repository.VideoStream.UpdateVideoStreamAsync(request, cancellationToken).ConfigureAwait(false);

        if (ret is not null)
        {
            await Publisher.Publish(new UpdateVideoStreamEvent(ret), cancellationToken).ConfigureAwait(false);
        }

        return ret;
    }
}
