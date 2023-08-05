using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroupByStreamNumber(int StreamGroupNumber) : IRequest<StreamGroupDto?>;

public class GetStreamGroupByStreamNumberValidator : AbstractValidator<GetStreamGroupByStreamNumber>
{
    public GetStreamGroupByStreamNumberValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

internal class GetStreamGroupByStreamNumberHandler : BaseMediatorRequestHandler, IRequestHandler<GetStreamGroupByStreamNumber, StreamGroupDto?>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetStreamGroupByStreamNumberHandler(IHttpContextAccessor httpContextAccessor, ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<StreamGroupDto?> Handle(GetStreamGroupByStreamNumber request, CancellationToken cancellationToken = default)
    {
        string url = _httpContextAccessor.GetUrl();
        StreamGroupDto? streamGroup = await Repository.StreamGroup.GetStreamGroupDtoByStreamGroupNumber(request.StreamGroupNumber, url, cancellationToken).ConfigureAwait(false);

        return streamGroup;
    }
}
