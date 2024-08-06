using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Models;

using System.Text.Json;

namespace StreamMaster.Application.StreamGroups.QueriesOld;

[RequireAll]
public record GetStreamGroupDiscover(int StreamGroupProfileId) : IRequest<string>;


[LogExecutionTimeAspect]
public class GetStreamGroupDiscoverHandler(IHttpContextAccessor httpContextAccessor, IStreamGroupService streamGroupService, IRepositoryWrapper Repository)
    : IRequestHandler<GetStreamGroupDiscover, string>
{

    public async Task<string> Handle(GetStreamGroupDiscover request, CancellationToken cancellationToken)
    {
        string url = httpContextAccessor.GetUrlWithPath();

        int maxTuners = await Repository.M3UFile.GetM3UMaxStreamCount();
        Discover discover = new(url, request.StreamGroupProfileId, maxTuners);

        StreamGroup? streamGroup = await streamGroupService.GetStreamGroupFromSGProfileIdAsync(request.StreamGroupProfileId);
        if (streamGroup == null)
        {
            return "";
        }
        discover.DeviceID = streamGroup.DeviceID;

        string jsonString = JsonSerializer.Serialize(discover, new JsonSerializerOptions { WriteIndented = true });
        return jsonString;
    }


}