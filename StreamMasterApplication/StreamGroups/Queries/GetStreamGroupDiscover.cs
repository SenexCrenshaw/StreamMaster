using FluentValidation;

using MediatR;

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

    public GetStreamGroupDiscoverHandler(
            ISender sender)
    {
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

        Discover discover = new(setting, $"{setting.BaseHostURL}api/streamgroups/{command.StreamGroupNumber}", command.StreamGroupNumber);

        string jsonString = JsonSerializer.Serialize(discover, new JsonSerializerOptions { WriteIndented = true });
        return jsonString;
    }
}
