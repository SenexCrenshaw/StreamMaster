using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.M3UFiles.Commands;

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

public class GetStreamGroupLineUpStatusHandler : BaseRequestHandler, IRequestHandler<GetStreamGroupLineUpStatus, string>
{

    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetStreamGroupLineUpStatusHandler(ILogger<ChangeM3UFileNameRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public Task<string> Handle(GetStreamGroupLineUpStatus request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId > 1)
        {
            IQueryable<StreamGroup> streamGroupExists = Repository.StreamGroup.GetAllStreamGroups().Where(x => x.Id == request.StreamGroupId);
            if (!streamGroupExists.Any())
            {
                return Task.FromResult("");
            }
        }

        string jsonString = JsonSerializer.Serialize(new LineupStatus(), new JsonSerializerOptions { WriteIndented = true });

        return Task.FromResult(jsonString);
    }
}
