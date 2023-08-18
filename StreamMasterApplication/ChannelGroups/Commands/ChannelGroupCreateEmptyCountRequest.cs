using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

namespace StreamMasterApplication.ChannelGroups.Commands;

public record ChannelGroupCreateEmptyCountRequest(int ChannelGroupId) : IRequest
{
}

public class ChannelGroupCreateEmptyCountRequestValidator : AbstractValidator<ChannelGroupCreateEmptyCountRequest>
{
    public ChannelGroupCreateEmptyCountRequestValidator()
    {
        _ = RuleFor(v => v.ChannelGroupId).NotNull().GreaterThan(0);
    }
}

public class ChannelGroupCreateEmptyCountRequestHandler : BaseMediatorRequestHandler, IRequestHandler<ChannelGroupCreateEmptyCountRequest>
{



    public ChannelGroupCreateEmptyCountRequestHandler(ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {

    }

    public async Task Handle(ChannelGroupCreateEmptyCountRequest request, CancellationToken cancellationToken)
    {
        await Repository.ChannelGroup.ChannelGroupCreateEmptyCount(request.ChannelGroupId).ConfigureAwait(false);

    }
}
