using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Models;

using System.Text.Json;

namespace StreamMaster.Application.StreamGroups.QueriesOld;

[RequireAll]
public record GetStreamGroupDiscover(int StreamGroupId, int StreamGroupProfileId) : IRequest<string>;

public class GetStreamGroupDiscoverValidator : AbstractValidator<GetStreamGroupDiscover>
{
    public GetStreamGroupDiscoverValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

[LogExecutionTimeAspect]
public class GetStreamGroupDiscoverHandler(ILogger<GetStreamGroupDiscover> logger, IHttpContextAccessor httpContextAccessor, IRepositoryWrapper Repository)
    : IRequestHandler<GetStreamGroupDiscover, string>
{

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

        string url = httpContextAccessor.GetUrlWithPath();

        int maxTuners = await Repository.M3UFile.GetM3UMaxStreamCount();
        Discover discover = new(url, request.StreamGroupId, maxTuners);

        string jsonString = JsonSerializer.Serialize(discover, new JsonSerializerOptions { WriteIndented = true });
        return jsonString;
    }


}