﻿using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.Common.Models;

using System.Text.Json;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupDiscover(int StreamGroupId) : IRequest<string>;

public class GetStreamGroupDiscoverValidator : AbstractValidator<GetStreamGroupDiscover>
{
    public GetStreamGroupDiscoverValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

[LogExecutionTimeAspect]
public class GetStreamGroupDiscoverHandler : BaseMediatorRequestHandler, IRequestHandler<GetStreamGroupDiscover, string>
{
    private readonly IHttpContextAccessor _httpContextAccessor;


    public GetStreamGroupDiscoverHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupDiscover> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: base(logger, repository, mapper, publisher, sender, hubContext) { _httpContextAccessor = httpContextAccessor; }


    public async Task<string> Handle(GetStreamGroupDiscover request, CancellationToken cancellationToken)
    {

        if (request.StreamGroupId > 1)
        {
            bool streamGroup = Repository.StreamGroup.FindAll().Any(a => a.Id == request.StreamGroupId);
            if (!streamGroup)
            {
                return "";
            }
        }

        int maxTuners = await Repository.M3UFile.GetM3UMaxStreamCountAsync();
        Discover discover = new(_httpContextAccessor.GetUrl(), request.StreamGroupId, maxTuners);

        string jsonString = JsonSerializer.Serialize(discover, new JsonSerializerOptions { WriteIndented = true });
        return jsonString;
    }

    //private string GetUrl()
    //{
    //    HttpRequest request = _httpContextAccessor.HttpContext.Request;
    //    string scheme = request.Scheme;
    //    HostString host = request.Host;
    //    PathString path = request.Path;
    //    path = path.ToString().Replace("/discover.json", "");
    //    string url = $"{scheme}://{host}{path}";

    //    return url;
    //}
}
