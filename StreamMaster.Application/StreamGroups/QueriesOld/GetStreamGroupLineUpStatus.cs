using FluentValidation;

using StreamMaster.Application.Common.Models;

using System.Text.Json;

namespace StreamMaster.Application.StreamGroups.QueriesOld;

[RequireAll]
public record GetStreamGroupLineupStatus(int StreamGroupId) : IRequest<string>;

public class GetStreamGroupLineupStatusValidator : AbstractValidator<GetStreamGroupLineupStatus>
{
    public GetStreamGroupLineupStatusValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

[LogExecutionTimeAspect]
public class GetStreamGroupLineupStatusHandler(ILogger<GetStreamGroupLineupStatus> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetStreamGroupLineupStatus, string>
{
    public Task<string> Handle(GetStreamGroupLineupStatus request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId > 1)
        {
            IQueryable<StreamGroup> streamGroupExists = Repository.StreamGroup.GetQuery().Where(x => x.Id == request.StreamGroupId);
            if (!streamGroupExists.Any())
            {
                return Task.FromResult("");
            }
        }

        string jsonString = JsonSerializer.Serialize(new LineupStatus(), new JsonSerializerOptions { WriteIndented = true });

        return Task.FromResult(jsonString);
    }
}