using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Commands;

[RequireAll]
public record UpdateChannelGroupRequest(string GroupName, string? NewGroupName, bool? IsHidden, int? Rank, string? Regex) : IRequest<ChannelGroupDto?>
{
}

public class UpdateChannelGroupRequestValidator : AbstractValidator<UpdateChannelGroupRequest>
{
    public UpdateChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.GroupName).NotNull().NotEmpty();
        _ = RuleFor(v => v.Rank).NotNull().GreaterThan(0);
    }
}

public class UpdateChannelGroupRequestHandler : IRequestHandler<UpdateChannelGroupRequest, ChannelGroupDto?>
{
    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublisher _publisher;

    public UpdateChannelGroupRequestHandler(
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher,
        IAppDbContext context
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _publisher = publisher;
        _context = context;
    }

    public async Task<ChannelGroupDto?> Handle(UpdateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        string url = _httpContextAccessor.GetUrl();
        var (cg, distinctList, streamGroups) = await _context.UpdateChannelGroup(request, url, cancellationToken).ConfigureAwait(false);

        if (distinctList != null && distinctList.Any())
        {
            await _publisher.Publish(new UpdateVideoStreamsEvent(distinctList), cancellationToken).ConfigureAwait(false);
        }

        if (streamGroups != null && streamGroups.Any())
        {
            foreach (var streamGroup in streamGroups.Where(a => a is not null))
            {
                await _publisher.Publish(new StreamGroupUpdateEvent(streamGroup), cancellationToken).ConfigureAwait(false);
                //var streamGroup = await _context.GetStreamGroupDto(ret.Id, url, cancellationToken).ConfigureAwait(false);
                //if (streamGroup is not null && streamGroup.ChildVideoStreams.Any())
                //{
                //    await _publisher.Publish(new UpdateVideoStreamsEvent(streamGroup.ChildVideoStreams), cancellationToken).ConfigureAwait(false);
                //}
            }
        }

        if ( cg is not null)
        {
            await _publisher.Publish(new UpdateChannelGroupEvent(cg), cancellationToken).ConfigureAwait(false);
        }

        return cg;
    }
}
