using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Attributes;

using System.Xml.Serialization;

using static StreamMasterDomain.Common.GetStreamGroupEPGHandler;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupCapability(int StreamGroupNumber) : IRequest<string>;

public class GetStreamGroupCapabilityValidator : AbstractValidator<GetStreamGroupCapability>
{
    public GetStreamGroupCapabilityValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public class GetStreamGroupCapabilityHandler : IRequestHandler<GetStreamGroupCapability, string>
{
    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetStreamGroupCapabilityHandler(
        IHttpContextAccessor httpContextAccessor,
        IAppDbContext context
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public async Task<string> Handle(GetStreamGroupCapability request, CancellationToken cancellationToken)
    {
        var url = GetUrl();

        if (request.StreamGroupNumber > 0)
        {
            var streamGroup = await _context.GetStreamGroupDtoByStreamGroupNumber(request.StreamGroupNumber, url, cancellationToken).ConfigureAwait(false);
            if (streamGroup == null)
            {
                return "";
            }
        }
        var settings = FileUtil.GetSetting();

        Capability capability = new(url, $"{settings.DeviceID}-{request.StreamGroupNumber}");

        using Utf8StringWriter textWriter = new();
        XmlSerializer serializer = new(typeof(Capability));
        serializer.Serialize(textWriter, capability);

        return textWriter.ToString();
    }

    private string GetUrl()
    {
        var request = _httpContextAccessor.HttpContext.Request;
        var scheme = request.Scheme;
        var host = request.Host;
        var path = request.Path;
        path = path.ToString().Replace("/capability", "");
        path = path.ToString().Replace("/device.xml", "");
        var url = $"{scheme}://{host}{path}";

        return url;
    }
}
