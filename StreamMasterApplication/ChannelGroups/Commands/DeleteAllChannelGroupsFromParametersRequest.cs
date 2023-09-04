using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.ChannelGroups.Commands;

public record DeleteAllChannelGroupsFromParametersRequest(ChannelGroupParameters Parameters) : IRequest
{
}

public class DeleteAllChannelGroupsFromParametersRequestValidator : AbstractValidator<DeleteAllChannelGroupsFromParametersRequest>
{
    public DeleteAllChannelGroupsFromParametersRequestValidator()
    {
        _ = RuleFor(v => v.Parameters).NotNull().NotEmpty();
    }
}

public class DeleteAllChannelGroupsFromParametersRequestHandler : BaseMemoryRequestHandler, IRequestHandler<DeleteAllChannelGroupsFromParametersRequest>
{

    public DeleteAllChannelGroupsFromParametersRequestHandler(ILogger<DeleteAllChannelGroupsFromParametersRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }
    public async Task Handle(DeleteAllChannelGroupsFromParametersRequest request, CancellationToken cancellationToken)
    {
        List<int> ids = await Repository.ChannelGroup.DeleteAllChannelGroupsFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);

        if (ids.Any())
        {
            foreach (int id in ids)
            {
                MemoryCache.RemoveChannelGroupStreamCount(id);
            }
            //await Publisher.Publish(new UpdateChannelGroupEvent(), cancellationToken);
            //await Publisher.Publish(new UpdateVideoStreamEvent(), cancellationToken);
        }
    }
}
