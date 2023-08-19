using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Commands;

[RequireAll]
public record UpdateChannelGroupsRequest(List<UpdateChannelGroupRequest> ChannelGroupRequests) : IRequest
{
}

public class UpdateChannelGroupsRequestValidator : AbstractValidator<UpdateChannelGroupsRequest>
{
}

public class UpdateChannelGroupsRequestHandler : BaseMediatorRequestHandler, IRequestHandler<UpdateChannelGroupsRequest>
{
    public UpdateChannelGroupsRequestHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task Handle(UpdateChannelGroupsRequest requests, CancellationToken cancellationToken)
    {
        //List<VideoStreamDto> results = new();

        foreach (UpdateChannelGroupRequest request in requests.ChannelGroupRequests)
        {

            await Sender.Send(new UpdateChannelGroupRequest(request.ChannelGroupName, request.NewGroupName, request.IsHidden, request.Rank), cancellationToken).ConfigureAwait(false);

        }

        //await Publisher.Publish(new UpdateChannelGroupsEvent(), cancellationToken).ConfigureAwait(false);

        //if (results.Any())
        //{
        //    await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);
        //}

    }
}