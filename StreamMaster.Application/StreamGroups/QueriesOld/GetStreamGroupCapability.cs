using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Models;

using System.Xml.Serialization;

using static StreamMaster.Domain.Common.GetStreamGroupEPGHandler;

namespace StreamMaster.Application.StreamGroups.QueriesOld;

[RequireAll]
public record GetStreamGroupCapability(int StreamGroupId, int StreamGroupProfileId) : IRequest<string>;

[LogExecutionTimeAspect]
public class GetStreamGroupCapabilityHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupCapability> logger, IRepositoryWrapper Repository, IOptionsMonitor<Setting> intSettings)
    : IRequestHandler<GetStreamGroupCapability, string>
{
    private readonly Setting settings = intSettings.CurrentValue;

    public async Task<string> Handle(GetStreamGroupCapability request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId > 1)
        {
            bool streamGroup = await Repository.StreamGroup.GetStreamGroupById(request.StreamGroupId).ConfigureAwait(false) != null;
            if (!streamGroup)
            {
                return "";
            }
        }

        Capability capability = new(GetUrl(), $"{settings.DeviceID}-{request.StreamGroupId}");

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
