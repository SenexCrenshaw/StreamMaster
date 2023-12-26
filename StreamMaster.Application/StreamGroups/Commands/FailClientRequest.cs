using FluentValidation;

using MediatR;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.Application.StreamGroups.Commands;

[RequireAll]
public record FailClientRequest(Guid clientId) : IRequest
{
}

public class FailClientRequestValidator : AbstractValidator<FailClientRequest>
{
    public FailClientRequestValidator()
    {
        _ = RuleFor(v => v.clientId)
           .NotNull()
           .NotEmpty();
    }
}

public class FailClientRequestHandler : IRequestHandler<FailClientRequest>
{
    private readonly IChannelManager _channelManager;

    public FailClientRequestHandler(IChannelManager channelManager)
    {
        _channelManager = channelManager;
    }

    public Task Handle(FailClientRequest request, CancellationToken cancellationToken)
    {
        //var clientId = Guid.Parse(request.clientId);

        _channelManager.FailClient(request.clientId);
        return Task.CompletedTask;
    }
}
