using FluentValidation;

using StreamMasterApplication.Common.Models;

using System.Text.Json;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupLineUpStatus(int StreamGroupId) : IRequest<string>;

public class GetStreamGroupLineUpStatusValidator : AbstractValidator<GetStreamGroupLineUpStatus>
{
    public GetStreamGroupLineUpStatusValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

[LogExecutionTimeAspect]
public class GetStreamGroupLineUpStatusHandler(ILogger<GetStreamGroupLineUpStatus> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetStreamGroupLineUpStatus, string>
{
    public Task<string> Handle(GetStreamGroupLineUpStatus request, CancellationToken cancellationToken)
    {

        if (request.StreamGroupId > 1)
        {
            IQueryable<StreamGroup> streamGroupExists = Repository.StreamGroup.GetStreamGroupQuery().Where(x => x.Id == request.StreamGroupId);
            if (!streamGroupExists.Any())
            {
                return Task.FromResult("");
            }
        }

        string jsonString = JsonSerializer.Serialize(new LineupStatus(), new JsonSerializerOptions { WriteIndented = true });

        return Task.FromResult(jsonString);
    }
}
