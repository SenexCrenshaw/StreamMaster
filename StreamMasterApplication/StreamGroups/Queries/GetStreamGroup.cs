using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroup(int Id) : IRequest<StreamGroupDto?>;

internal class GetStreamGroupHandler : BaseMediatorRequestHandler, IRequestHandler<GetStreamGroup, StreamGroupDto?>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetStreamGroupHandler(IHttpContextAccessor httpContextAccessor, ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<StreamGroupDto?> Handle(GetStreamGroup request, CancellationToken cancellationToken = default)
    {
        if (request.Id == 0) return new StreamGroupDto { Id = 0, Name = "All" };

        string url = _httpContextAccessor.GetUrl();
        StreamGroupDto? streamGroup = await Repository.StreamGroup.GetStreamGroupDto(request.Id, url, cancellationToken).ConfigureAwait(false);
        return streamGroup;
    }
}
