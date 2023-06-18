using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Attributes;

using System.Text.Json;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupDiscover(int StreamGroupNumber) : IRequest<string>;

public class GetStreamGroupDiscoverValidator : AbstractValidator<GetStreamGroupDiscover>
{
    public GetStreamGroupDiscoverValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public class GetStreamGroupDiscoverHandler : IRequestHandler<GetStreamGroupDiscover, string>
{
    private readonly ISender _sender;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetStreamGroupDiscoverHandler(
           IHttpContextAccessor httpContextAccessor,
            ISender sender
           )
    {
        _httpContextAccessor = httpContextAccessor;
        _sender = sender;
    }

    public async Task<string> Handle(GetStreamGroupDiscover command, CancellationToken cancellationToken)
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
        Discover discover = new(setting, url, command.StreamGroupNumber);

        string jsonString = JsonSerializer.Serialize(discover, new JsonSerializerOptions { WriteIndented = true });
        return jsonString;
    }

    private string GetUrl()
    {
        var request = _httpContextAccessor.HttpContext.Request;
        var scheme = request.Scheme;
        var host = request.Host;
        var path = request.Path;
        path = path.ToString().Replace("/discover.json", "");
        var url = $"{scheme}://{host}{path}";

        return url;
    }
}
