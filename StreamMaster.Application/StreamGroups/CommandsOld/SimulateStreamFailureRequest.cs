using FluentValidation;

using MediatR;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.Application.StreamGroups.Commands;

[RequireAll]
public record SimulateStreamFailureRequest(string StreamUrl) : IRequest
{
}

public class SimulateStreamFailureRequestValidator : AbstractValidator<SimulateStreamFailureRequest>
{
    public SimulateStreamFailureRequestValidator()
    {
        _ = RuleFor(v => v.StreamUrl)
           .NotNull()
           .NotEmpty();
    }
}

public class SimulateStreamFailureRequestHandler : IRequestHandler<SimulateStreamFailureRequest>
{
    private readonly IChannelManager _channelManager;

    public SimulateStreamFailureRequestHandler(IChannelManager channelManager)
    {
        _channelManager = channelManager;
    }

    public Task Handle(SimulateStreamFailureRequest request, CancellationToken cancellationToken)
    {
        _channelManager.SimulateStreamFailure(request.StreamUrl);
        return Task.CompletedTask;
    }
}
