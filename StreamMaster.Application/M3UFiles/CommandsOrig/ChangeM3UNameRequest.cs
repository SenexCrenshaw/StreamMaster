using FluentValidation;

namespace StreamMaster.Application.M3UFiles.CommandsOrig;

public record ChangeM3UFileNameRequest(int Id, string Name) : IRequest<M3UFileDto?> { }

public class ChangeM3UFileNameRequestValidator : AbstractValidator<ChangeM3UFileNameRequest>
{
    public ChangeM3UFileNameRequestValidator()
    {
        _ = RuleFor(v => v.Name)
            .NotNull()
            .MaximumLength(32)
            .NotEmpty();
    }
}

public class ChangeM3UFileNameRequestHandler(ILogger<ChangeM3UFileNameRequest> logger, IRepositoryWrapper Repository)
    : IRequestHandler<ChangeM3UFileNameRequest, M3UFileDto?>
{
    public async Task<M3UFileDto?> Handle(ChangeM3UFileNameRequest request, CancellationToken cancellationToken)
    {
        return await Repository.M3UFile.ChangeM3UFileName(request.Id, request.Name).ConfigureAwait(false);

    }
}
