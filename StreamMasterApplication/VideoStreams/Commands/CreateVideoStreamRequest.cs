using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.Commands;



public class CreateVideoStreamRequestValidator : AbstractValidator<CreateVideoStreamRequest>
{
    public CreateVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.Tvg_name).NotNull().NotEmpty();
    }
}

public class CreateVideoStreamRequestHandler : BaseMediatorRequestHandler, IRequestHandler<CreateVideoStreamRequest>
{

    public CreateVideoStreamRequestHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task Handle(CreateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        await Repository.VideoStream.CreateVideoStreamAsync(request, cancellationToken);
        await Publisher.Publish(new CreateVideoStreamEvent(), cancellationToken).ConfigureAwait(false);

    }
}
