using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

namespace StreamMasterApplication.ChannelGroups.Commands;

public record ChannelGroupsSetCountRequest(List<int> ChannelGroupIds, int toAdd) : IRequest
{
}

public class ChannelGroupsSetCountRequestValidator : AbstractValidator<ChannelGroupsSetCountRequest>
{
    public ChannelGroupsSetCountRequestValidator()
    {
        _ = RuleFor(v => v.ChannelGroupIds).NotNull().NotEmpty();
    }
}

public class ChannelGroupsSetCountRequestHandler : BaseMediatorRequestHandler, IRequestHandler<ChannelGroupsSetCountRequest>
{



    public ChannelGroupsSetCountRequestHandler(ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {

    }

    public async Task Handle(ChannelGroupsSetCountRequest request, CancellationToken cancellationToken)
    {
        await Repository.ChannelGroup.ChannelGroupsSetCount(request.ChannelGroupIds, request.toAdd).ConfigureAwait(false);

    }
}
