using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

namespace StreamMasterApplication.ChannelGroups.Commands;

public record ChannelGroupDeleteCountRequest(int ChannelGroupId, int toDelete, bool isHidden) : IRequest
{
}

public class ChannelGroupDeleteCountRequestValidator : AbstractValidator<ChannelGroupDeleteCountRequest>
{
    public ChannelGroupDeleteCountRequestValidator()
    {
        _ = RuleFor(v => v.ChannelGroupId).NotNull().GreaterThan(0);
    }
}

public class ChannelGroupDeleteCountRequestHandler : BaseMediatorRequestHandler, IRequestHandler<ChannelGroupDeleteCountRequest>
{



    public ChannelGroupDeleteCountRequestHandler(ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {

    }

    public async Task Handle(ChannelGroupDeleteCountRequest request, CancellationToken cancellationToken)
    {
        await Repository.ChannelGroup.ChannelGroupDeleteFromCount(request.ChannelGroupId, request.toDelete, request.isHidden).ConfigureAwait(false);

    }
}
