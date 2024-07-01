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


        string url = httpContextAccessor.GetUrlWithPath();

        int maxTuners = await Repository.M3UFile.GetM3UMaxStreamCount();
        Discover discover = new(url, request.StreamGroupId, maxTuners);
        if (request.StreamGroupId > 1)
        {
            var streamGroup = await Repository.StreamGroup.GetStreamGroupById(request.StreamGroupId).ConfigureAwait(false);
            if (streamGroup == null)
            {
                return "";
            }
            discover.DeviceID = streamGroup.DeviceID;
        }
        string jsonString = JsonSerializer.Serialize(discover, new JsonSerializerOptions { WriteIndented = true });
        return jsonString;
    }


}