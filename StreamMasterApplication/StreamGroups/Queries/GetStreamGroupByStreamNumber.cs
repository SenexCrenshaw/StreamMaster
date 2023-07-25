using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

using StreamMasterApplication.Common.Extensions;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroupByStreamNumber(int StreamGroupNumber) : IRequest<StreamGroupDto?>;

public class GetStreamGroupByStreamNumberValidator : AbstractValidator<GetStreamGroupByStreamNumber>
{
    public GetStreamGroupByStreamNumberValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

internal class GetStreamGroupByStreamNumberHandler : IRequestHandler<GetStreamGroupByStreamNumber, StreamGroupDto?>
{
    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetStreamGroupByStreamNumberHandler(
        IHttpContextAccessor httpContextAccessor,
        IAppDbContext context
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public async Task<StreamGroupDto?> Handle(GetStreamGroupByStreamNumber request, CancellationToken cancellationToken = default)
    {
        var url = _httpContextAccessor.GetUrl();
        var streamGroup = await _context.GetStreamGroupDtoByStreamGroupNumber(request.StreamGroupNumber, url, cancellationToken).ConfigureAwait(false);

        return streamGroup;
    }
}
