using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

namespace StreamMasterApplication.StreamGroups.Commands;

public class DeleteStreamGroupRequest : IRequest<int?>
{
    public int Id { get; set; }
}

public class DeleteStreamGroupRequestValidator : AbstractValidator<DeleteStreamGroupRequest>
{
    public DeleteStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

public class DeleteStreamGroupHandler : BaseMediatorRequestHandler, IRequestHandler<DeleteStreamGroupRequest, int?>
{

    private readonly IPublisher _publisher;

    public DeleteStreamGroupHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<int?> Handle(DeleteStreamGroupRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Id < 1)
        {
            return null;
        }

        if (await Repository.StreamGroup.DeleteStreamGroupsync(request.Id, cancellationToken))
        {
            await _publisher.Publish(new StreamGroupDeleteEvent(), cancellationToken).ConfigureAwait(false);
            return request.Id;
        }

        return null;
    }
}
