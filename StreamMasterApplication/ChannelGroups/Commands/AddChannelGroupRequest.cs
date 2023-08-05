using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Commands;

[RequireAll]
public record CreateChannelGroupRequest(string GroupName, int Rank, string? Regex) : IRequest<ChannelGroupDto?>
{
}

public class CreateChannelGroupRequestValidator : AbstractValidator<CreateChannelGroupRequest>
{
    public CreateChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.GroupName).NotNull().NotEmpty();
        _ = RuleFor(v => v.Rank).NotNull().GreaterThan(0);
    }
}

public class CreateChannelGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<CreateChannelGroupRequest, ChannelGroupDto?>
{

    public CreateChannelGroupRequestHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<ChannelGroupDto?> Handle(CreateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        if (await Repository.ChannelGroup.GetChannelGroupByNameAsync(request.GroupName).ConfigureAwait(false) == null)
        {
            return null;
        }

        ChannelGroup channelGroup = new() { Name = request.GroupName, Rank = request.Rank, IsReadOnly = false };
        if (!string.IsNullOrEmpty(request.Regex))
        {
            channelGroup.RegexMatch = request.Regex;
        }

        Repository.ChannelGroup.CreateChannelGroup(channelGroup);
        await Repository.SaveAsync().ConfigureAwait(false);

        ChannelGroupDto result = Mapper.Map<ChannelGroupDto>(channelGroup);
        await Publisher.Publish(new AddChannelGroupEvent(result), cancellationToken).ConfigureAwait(false);
        return result;
    }
}
