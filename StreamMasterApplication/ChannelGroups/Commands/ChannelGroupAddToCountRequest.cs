using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

namespace StreamMasterApplication.ChannelGroups.Commands;

public record ChannelGroupAddToCountRequest(int ChannelGroupId, int toAdd, bool isHidden) : IRequest
{
}

public class ChannelGroupAddToCountRequestValidator : AbstractValidator<ChannelGroupAddToCountRequest>
{
    public ChannelGroupAddToCountRequestValidator()
    {
        _ = RuleFor(v => v.ChannelGroupId).NotNull().GreaterThan(0);
    }
}

public class ChannelGroupAddToCountRequestHandler : BaseMediatorRequestHandler, IRequestHandler<ChannelGroupAddToCountRequest>
{



    public ChannelGroupAddToCountRequestHandler(ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {

    }

    public async Task Handle(ChannelGroupAddToCountRequest request, CancellationToken cancellationToken)
    {
        await Repository.ChannelGroup.ChannelGroupAddToCount(request.ChannelGroupId, request.toAdd, request.isHidden).ConfigureAwait(false);

    }
}
