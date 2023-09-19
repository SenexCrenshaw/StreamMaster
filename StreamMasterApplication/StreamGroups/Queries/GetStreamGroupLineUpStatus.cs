using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StreamMasterDomain.Authentication;
using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Common;
using StreamMasterDomain.Models;
using StreamMasterDomain.Services;

using System.Text.Json;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupLineUpStatus(int StreamGroupId) : IRequest<string>;

public class GetStreamGroupLineUpStatusValidator : AbstractValidator<GetStreamGroupLineUpStatus>
{
    public GetStreamGroupLineUpStatusValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

[LogExecutionTimeAspect]
public class GetStreamGroupLineUpStatusHandler : BaseMediatorRequestHandler, IRequestHandler<GetStreamGroupLineUpStatus, string>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetStreamGroupLineUpStatusHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupLineUpStatus> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { _httpContextAccessor = httpContextAccessor; }

    public async Task<string> Handle(GetStreamGroupLineUpStatus request, CancellationToken cancellationToken)
{

    if (request.StreamGroupId > 1)
    {
        IQueryable<StreamGroup> streamGroupExists = Repository.StreamGroup.GetStreamGroupQuery().Where(x => x.Id == request.StreamGroupId);
        if (!streamGroupExists.Any())
            {
                return "";
            }
        }
      
        string jsonString = JsonSerializer.Serialize(new LineupStatus(), new JsonSerializerOptions { WriteIndented = true });

        return jsonString;
    }
}
