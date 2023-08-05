using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Attributes;

using System.Text.Json;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupDiscover(int StreamGroupNumber) : IRequest<string>;

public class GetStreamGroupDiscoverValidator : AbstractValidator<GetStreamGroupDiscover>
{
    public GetStreamGroupDiscoverValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public class GetStreamGroupDiscoverHandler : IRequestHandler<GetStreamGroupDiscover, string>
{
    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private IRepositoryWrapper Repository { get; }
    public GetStreamGroupDiscoverHandler(IHttpContextAccessor httpContextAccessor, IAppDbContext context, IRepositoryWrapper repository)
    {
        Repository = repository;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(GetStreamGroupDiscover request, CancellationToken cancellationToken)
    {
        var url = GetUrl();
        if (request.StreamGroupNumber > 0)
        {
            var streamGroup = await _context.GetStreamGroupDtoByStreamGroupNumber(request.StreamGroupNumber, url, cancellationToken).ConfigureAwait(false);
            if (streamGroup == null)
            {
                return "";
            }
        }

        var maxTuners = await Repository.M3UFile.GetM3UMaxStreamCountAsync();
        Discover discover = new(url, request.StreamGroupNumber, maxTuners);

        string jsonString = JsonSerializer.Serialize(discover, new JsonSerializerOptions { WriteIndented = true });
        return jsonString;
    }

    private string GetUrl()
    {
        var request = _httpContextAccessor.HttpContext.Request;
        var scheme = request.Scheme;
        var host = request.Host;
        var path = request.Path;
        path = path.ToString().Replace("/discover.json", "");
        var url = $"{scheme}://{host}{path}";

        return url;
    }
}
