using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Application.Common.Models;

using System.Text.Json;

namespace StreamMaster.Application.StreamGroups.Queries;

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
    public GetStreamGroupDiscoverHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupDiscover> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { _httpContextAccessor = httpContextAccessor; }

    public async Task<string> Handle(GetStreamGroupDiscover request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId > 1)
        {
            bool streamGroup = await Repository.StreamGroup.GetStreamGroupById(request.StreamGroupId).ConfigureAwait(false) != null;
            if (!streamGroup)
            {
                return "";
            }
        }

        string url = _httpContextAccessor.GetUrlWithPath();

        int maxTuners = await Repository.M3UFile.GetM3UMaxStreamCount();
        Discover discover = new(url, request.StreamGroupId, maxTuners);

        string jsonString = JsonSerializer.Serialize(discover, new JsonSerializerOptions { WriteIndented = true });
        return jsonString;
    }


}