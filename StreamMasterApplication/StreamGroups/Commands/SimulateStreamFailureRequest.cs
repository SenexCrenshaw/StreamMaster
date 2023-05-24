using FluentValidation;

using MediatR;

using StreamMasterDomain.Attributes;

namespace StreamMasterApplication.StreamGroups.Commands;

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
    private readonly IRingBufferManager _ringBufferManager;

    public SimulateStreamFailureRequestHandler(IRingBufferManager ringBufferManager)
    {
        _ringBufferManager = ringBufferManager;
    }

    public Task Handle(SimulateStreamFailureRequest request, CancellationToken cancellationToken)
    {
        _ringBufferManager.SimulateStreamFailure(request.StreamUrl);
        return Task.CompletedTask;
    }
}
