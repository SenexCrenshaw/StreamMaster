using FluentValidation;

namespace StreamMaster.Application.SMStreams.Commands;

public record ToggleSMStreamVisibleRequest(string Id) : IRequest<bool> { }

public class ToggleSMStreamVisibleRequestValidator : AbstractValidator<ToggleSMStreamVisibleRequest>
{
    public ToggleSMStreamVisibleRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class ToggleSMStreamVisibleRequestHandler(ILogger<ToggleSMStreamVisibleRequest> logger, IRepositoryWrapper Repository)
    : IRequestHandler<ToggleSMStreamVisibleRequest, bool>
{
    public async Task<bool> Handle(ToggleSMStreamVisibleRequest request, CancellationToken cancellationToken)
    {
        SMStreamDto? stream = await Repository.SMStream.ToggleSMStreamVisibleById(request.Id, cancellationToken).ConfigureAwait(false);
        return stream != null && stream.IsHidden;
    }
}
