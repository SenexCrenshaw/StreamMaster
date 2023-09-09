using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMasterApplication.Common.Models;

using System.Xml.Serialization;

using static StreamMasterDomain.Common.GetStreamGroupEPGHandler;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupCapability(int StreamGroupId) : IRequest<string>;

public class GetStreamGroupCapabilityValidator : AbstractValidator<GetStreamGroupCapability>
{
    public GetStreamGroupCapabilityValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public class GetStreamGroupCapabilityHandler : BaseMediatorRequestHandler, IRequestHandler<GetStreamGroupCapability, string>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetStreamGroupCapabilityHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupCapability> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: base(logger, repository, mapper, publisher, sender, hubContext) { _httpContextAccessor = httpContextAccessor; }


    public Task<string> Handle(GetStreamGroupCapability request, CancellationToken cancellationToken)
    {

        if (request.StreamGroupId > 1)
        {
            bool streamGroup = Repository.StreamGroup.FindAll().Any(a => a.Id == request.StreamGroupId);
            if (!streamGroup)
            {
                return Task.FromResult("");
            }
        }
        Setting settings = FileUtil.GetSetting();

        Capability capability = new(GetUrl(), $"{settings.DeviceID}-{request.StreamGroupId}");

        using Utf8StringWriter textWriter = new();
        XmlSerializer serializer = new(typeof(Capability));
        serializer.Serialize(textWriter, capability);

        return Task.FromResult(textWriter.ToString());
    }

    private string GetUrl()
    {
        HttpRequest request = _httpContextAccessor.HttpContext.Request;
        string scheme = request.Scheme;
        HostString host = request.Host;
        PathString path = request.Path;
        path = path.ToString().Replace("/capability", "");
        path = path.ToString().Replace("/device.xml", "");
        string url = $"{scheme}://{host}{path}";

        return url;
    }
}
