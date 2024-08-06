using StreamMaster.Application.Common.Models;

using System.Text.Json;

namespace StreamMaster.Application.StreamGroups.QueriesOld;

[RequireAll]
public record GetStreamGroupLineupStatus() : IRequest<string>;

[LogExecutionTimeAspect]
public class GetStreamGroupLineupStatusHandler() : IRequestHandler<GetStreamGroupLineupStatus, string>
{
    public Task<string> Handle(GetStreamGroupLineupStatus request, CancellationToken cancellationToken)
    {
        string jsonString = JsonSerializer.Serialize(new LineupStatus(), new JsonSerializerOptions { WriteIndented = true });

        return Task.FromResult(jsonString);
    }
}