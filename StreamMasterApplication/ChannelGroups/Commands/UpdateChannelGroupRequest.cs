using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.M3UFiles.Commands;
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

public class UpdateChannelGroupRequestHandler : BaseDBRequestHandler, IRequestHandler<UpdateChannelGroupRequest, ChannelGroupDto?>
{

    private readonly IHttpContextAccessor _httpContextAccessor;


    public UpdateChannelGroupRequestHandler(IHttpContextAccessor httpContextAccessor, IAppDbContext context, ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, context, memoryCache)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ChannelGroupDto?> Handle(UpdateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        var videoStreamsRepo = await Repository.VideoStream.GetAllVideoStreamsAsync().ConfigureAwait(false);

        var originalStreamsIds = videoStreamsRepo.Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == request.GroupName.ToLower()).Select(a => a.Id);

        string url = _httpContextAccessor.GetUrl();
        var (cg, distinctList, streamGroups) = await Context.UpdateChannelGroup(request, url, cancellationToken).ConfigureAwait(false);

        if (distinctList != null && distinctList.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(distinctList), cancellationToken).ConfigureAwait(false);
        }

        if (streamGroups != null && streamGroups.Any())
        {
            foreach (var streamGroup in streamGroups.Where(a => a is not null))
            {
                await Publisher.Publish(new StreamGroupUpdateEvent(streamGroup), cancellationToken).ConfigureAwait(false);
                //var streamGroup = await _context.GetStreamGroupDto(ret.Id, url, cancellationToken).ConfigureAwait(false);
                //if (streamGroup is not null && streamGroup.ChildVideoStreams.Any())
                //{
                //    await _publisher.Publish(new UpdateVideoStreamsEvent(streamGroup.ChildVideoStreams), cancellationToken).ConfigureAwait(false);
                //}
            }
        }

        if (originalStreamsIds.Any())
        {

            var orginalStreams = Repository.VideoStream.GetVideoStreamsByMatchingIds(originalStreamsIds);
            var originalStreamsDto = Mapper.Map<List<VideoStreamDto>>(orginalStreams);
            await Publisher.Publish(new UpdateVideoStreamsEvent(originalStreamsDto), cancellationToken).ConfigureAwait(false);
        }

        if (cg is not null)
        {
            await Publisher.Publish(new UpdateChannelGroupEvent(cg), cancellationToken).ConfigureAwait(false);
        }

        return cg;
    }
}
