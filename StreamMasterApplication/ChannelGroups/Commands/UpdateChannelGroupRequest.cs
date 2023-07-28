using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public UpdateChannelGroupRequestHandler(
        IHttpContextAccessor httpContextAccessor,

        IMapper mapper,
        IPublisher publisher,
        IAppDbContext context
    )
    {
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _publisher = publisher;
        _context = context;
    }

    public async Task<ChannelGroupDto?> Handle(UpdateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        var originalStreamsIds = await _context.VideoStreams.Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == request.GroupName.ToLower()).Select(a => a.Id).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

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

        if (originalStreamsIds.Any())
        {
            var orginalStreams = await _context.VideoStreams.Where(a => originalStreamsIds.Contains(a.Id)).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            var originalStreamsDto = _mapper.Map<List<VideoStreamDto>>(orginalStreams);
            await _publisher.Publish(new UpdateVideoStreamsEvent(originalStreamsDto), cancellationToken).ConfigureAwait(false);
        }

        if (cg is not null)
        {
            await _publisher.Publish(new UpdateChannelGroupEvent(cg), cancellationToken).ConfigureAwait(false);
        }

        return cg;
    }
}
