using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

namespace StreamMasterApplication.ChannelGroups.Commands;

public record ChannelGroupSetCountRequest(int ChannelGroupId, int toAdd) : IRequest
{
}

public class ChannelGroupSetCountRequestValidator : AbstractValidator<ChannelGroupSetCountRequest>
{
    public ChannelGroupSetCountRequestValidator()
    {
        _ = RuleFor(v => v.ChannelGroupId).NotNull().GreaterThan(0);
    }
}

public class ChannelGroupSetCountRequestHandler : BaseMediatorRequestHandler, IRequestHandler<ChannelGroupSetCountRequest>
{



    public ChannelGroupSetCountRequestHandler(ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {

    }

    public async Task Handle(ChannelGroupSetCountRequest request, CancellationToken cancellationToken)
    {
        await Repository.ChannelGroup.ChannelGroupSetCount(request.ChannelGroupId, request.toAdd).ConfigureAwait(false);

    }
}
