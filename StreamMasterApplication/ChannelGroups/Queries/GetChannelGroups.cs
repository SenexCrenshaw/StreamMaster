using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

using System;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupsQuery(ChannelGroupParameters Parameters) : IRequest<PagedResponse<ChannelGroupDto>>;

internal class GetChannelGroupsQueryHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupsQuery, PagedResponse<ChannelGroupDto>>
{
    public GetChannelGroupsQueryHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<PagedResponse<ChannelGroupDto>> Handle(GetChannelGroupsQuery request, CancellationToken cancellationToken)
    {
        return await Repository.ChannelGroup.GetChannelGroupsAsync(request.Parameters).ConfigureAwait(false);
    }
}