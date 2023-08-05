using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupsQuery() : IRequest<List<ChannelGroupDto>>;

internal class GetChannelGroupsQueryHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupsQuery, List<ChannelGroupDto>>
{

    public GetChannelGroupsQueryHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public Task<List<ChannelGroupDto>> Handle(GetChannelGroupsQuery request, CancellationToken cancellationToken)
    {
        List<ChannelGroupDto> ret = Mapper.Map<List<ChannelGroupDto>>(Repository.ChannelGroup.GetAllChannelGroups());
        return Task.FromResult(ret);

    }
}
