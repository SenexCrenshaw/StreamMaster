using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Attributes;

using System.Text.Json;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupLineUpStatus(int StreamGroupNumber) : IRequest<string>;

public class GetStreamGroupLineUpStatusValidator : AbstractValidator<GetStreamGroupLineUpStatus>
{
    public GetStreamGroupLineUpStatusValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public class GetStreamGroupLineUpStatusHandler : IRequestHandler<GetStreamGroupLineUpStatus, string>
{
    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetStreamGroupLineUpStatusHandler(
         IHttpContextAccessor httpContextAccessor,
        IAppDbContext context
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public async Task<string> Handle(GetStreamGroupLineUpStatus request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupNumber > 0)
        {
            var streamGroupExists = await _context.StreamGroups.AnyAsync(x => x.StreamGroupNumber == request.StreamGroupNumber, cancellationToken).ConfigureAwait(false);
            if (!streamGroupExists)
            {
                return "";
            }
        }

        string jsonString = JsonSerializer.Serialize(new LineupStatus(), new JsonSerializerOptions { WriteIndented = true });

        return jsonString;
    }
}
