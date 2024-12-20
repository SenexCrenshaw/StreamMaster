using FluentValidation;

namespace StreamMaster.Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            ValidationContext<TRequest> context = new(request);

            FluentValidation.Results.ValidationResult[] validationResults = await Task.WhenAll(
                validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);

            List<FluentValidation.Results.ValidationFailure> failures = [.. validationResults
                .Where(r => r.Errors.Count != 0)
                .SelectMany(r => r.Errors)];

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }
        return await next().ConfigureAwait(false);
    }
}
