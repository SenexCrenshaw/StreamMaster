using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupByName(string Name) : IRequest<ChannelGroupDto?>;

internal class GetChannelGroupByNameHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupByName, ChannelGroupDto?>
{

    public GetChannelGroupByNameHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<ChannelGroupDto?> Handle(GetChannelGroupByName request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupByNameAsync(request.Name).ConfigureAwait(false);

        if (channelGroup == null)
        {
            return null;
        }

        ChannelGroupDto ret = Mapper.Map<ChannelGroupDto>(channelGroup);

        return ret;
    }
}
