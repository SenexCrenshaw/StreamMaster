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
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISender _sender;

    public GetStreamGroupCapabilityHandler(
        IHttpContextAccessor httpContextAccessor,
            ISender sender
           )
    {
        _httpContextAccessor = httpContextAccessor;
        _sender = sender;
    }

    public async Task<string> Handle(GetStreamGroupCapability command, CancellationToken cancellationToken)
    {

        if (command.StreamGroupNumber > 0)
        {
            StreamMasterDomain.Dto.StreamGroupDto? sg = await _sender.Send(new GetStreamGroupByStreamNumber(command.StreamGroupNumber), cancellationToken).ConfigureAwait(false);
            if (sg == null)
            {
                return "";
            }
        }

        StreamMasterDomain.Dto.SettingDto setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);

        var url = GetUrl();

        Capability capability = new(url, $"{setting.DeviceID}-{command.StreamGroupNumber}");

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
