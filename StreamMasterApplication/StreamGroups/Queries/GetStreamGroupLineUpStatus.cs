using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.M3UFiles.Commands;

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

public class GetStreamGroupLineUpStatusHandler : BaseRequestHandler, IRequestHandler<GetStreamGroupLineUpStatus, string>
{

    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetStreamGroupLineUpStatusHandler(ILogger<ChangeM3UFileNameRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public Task<string> Handle(GetStreamGroupLineUpStatus request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupNumber > 0)
        {
            IQueryable<StreamGroup> streamGroupExists = Repository.StreamGroup.GetAllStreamGroups().Where(x => x.StreamGroupNumber == request.StreamGroupNumber);
            if (!streamGroupExists.Any())
            {
                return Task.FromResult("");
            }
        }

        string jsonString = JsonSerializer.Serialize(new LineupStatus(), new JsonSerializerOptions { WriteIndented = true });

        return Task.FromResult(jsonString);
    }
}
