using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

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

public class GetStreamGroupDiscoverHandler : BaseMediatorRequestHandler, IRequestHandler<GetStreamGroupDiscover, string>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetStreamGroupDiscoverHandler(IHttpContextAccessor httpContextAccessor, ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task<string> Handle(GetStreamGroupDiscover request, CancellationToken cancellationToken)
    {
        string url = GetUrl();
        if (request.StreamGroupNumber > 0)
        {
            StreamGroupDto? streamGroup = await Repository.StreamGroup.GetStreamGroupDtoByStreamGroupNumber(request.StreamGroupNumber, url, cancellationToken).ConfigureAwait(false);
            if (streamGroup == null)
            {
                return "";
            }
        }

        int maxTuners = await Repository.M3UFile.GetM3UMaxStreamCountAsync();
        Discover discover = new(url, request.StreamGroupNumber, maxTuners);

        string jsonString = JsonSerializer.Serialize(discover, new JsonSerializerOptions { WriteIndented = true });
        return jsonString;
    }

    private string GetUrl()
    {
        HttpRequest request = _httpContextAccessor.HttpContext.Request;
        string scheme = request.Scheme;
        HostString host = request.Host;
        PathString path = request.Path;
        path = path.ToString().Replace("/discover.json", "");
        string url = $"{scheme}://{host}{path}";

        return url;
    }
}
