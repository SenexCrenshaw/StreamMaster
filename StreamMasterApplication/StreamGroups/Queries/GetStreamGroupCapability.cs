using Microsoft.AspNetCore.Http;

using StreamMasterApplication.Common.Models;

using System.Xml.Serialization;

using static StreamMasterDomain.Common.GetStreamGroupEPGHandler;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupCapability(int StreamGroupId) : IRequest<string>;

[LogExecutionTimeAspect]
public class GetStreamGroupCapabilityHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupCapability> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext), IRequestHandler<GetStreamGroupCapability, string>
{
    public async Task<string> Handle(GetStreamGroupCapability request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId > 1)
        {
            bool streamGroup = Repository.StreamGroup.FindAll().Any(a => a.Id == request.StreamGroupId);
            if (!streamGroup)
            {
                return "";
            }
        }
        Setting setting = await GetSettingsAsync();

        Capability capability = new(GetUrl(), $"{setting.DeviceID}-{request.StreamGroupId}");

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
