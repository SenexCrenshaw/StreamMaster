using AutoMapper;

using FluentValidation;

using MediatR;

using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Attributes;

using System.Text.Json;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupLineUpStatus(int StreamGroupNumber) : IRequest<string>;

public class GetStreamGroupLineUpStatusValidator : AbstractValidator<GetStreamGroupLineUpStatus>
{
    public GetStreamGroupLineUpStatusValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public class GetStreamGroupLineUpStatusHandler : IRequestHandler<GetStreamGroupLineUpStatus, string>
{
    private readonly IAppDbContext _context;

    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public GetStreamGroupLineUpStatusHandler(
            IMapper mapper,
            ISender sender,
            IAppDbContext context)
    {
        _mapper = mapper;
        _context = context;
        _sender = sender;
    }

    public async Task<string> Handle(GetStreamGroupLineUpStatus command, CancellationToken cancellationToken)
    {
        if (command.StreamGroupNumber > 0)
        {
            StreamMasterDomain.Dto.StreamGroupDto? sg = await _sender.Send(new GetStreamGroupByStreamNumber(command.StreamGroupNumber), cancellationToken).ConfigureAwait(false);
            if (sg == null)
            {
                return "";
            }
        }

        string jsonString = JsonSerializer.Serialize(new LineupStatus(), new JsonSerializerOptions { WriteIndented = true });

        return jsonString;
    }
}
