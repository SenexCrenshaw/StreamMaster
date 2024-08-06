using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Models;

using System.Xml.Serialization;

using static StreamMaster.Domain.Common.GetStreamGroupEPGHandler;

namespace StreamMaster.Application.StreamGroups.QueriesOld;

[RequireAll]
public record GetStreamGroupCapability(int StreamGroupProfileId) : IRequest<string>;

[LogExecutionTimeAspect]
public class GetStreamGroupCapabilityHandler(IHttpContextAccessor httpContextAccessor, IStreamGroupService streamGroupService)
    : IRequestHandler<GetStreamGroupCapability, string>
{

    public async Task<string> Handle(GetStreamGroupCapability request, CancellationToken cancellationToken)
    {

        StreamGroup streamGroup = await streamGroupService.GetStreamGroupFromSGProfileIdAsync(request.StreamGroupProfileId).ConfigureAwait(false);

        Capability capability = new(GetUrl(), $"{streamGroup.DeviceID}-{request.StreamGroupProfileId}");

        using Utf8StringWriter textWriter = new();
        XmlSerializer serializer = new(typeof(Capability));
        serializer.Serialize(textWriter, capability);

        return textWriter.ToString();
    }

    private string GetUrl()
    {
        HttpRequest request = httpContextAccessor.HttpContext.Request;
        string scheme = request.Scheme;
        HostString host = request.Host;
        PathString path = request.Path;
        path = path.ToString().Replace("/capability", "");
        path = path.ToString().Replace("/device.xml", "");
        string url = $"{scheme}://{host}{path}";

        return url;
    }
}
