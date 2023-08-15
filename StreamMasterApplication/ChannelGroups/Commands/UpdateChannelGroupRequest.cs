using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Commands;



public class UpdateChannelGroupRequestValidator : AbstractValidator<UpdateChannelGroupRequest>
{
    public UpdateChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.ChannelGroupName).NotNull().NotEmpty();
        _ = RuleFor(v => v.Rank).NotNull().GreaterThan(0);
    }
}

public class UpdateChannelGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<UpdateChannelGroupRequest, ChannelGroupDto?>
{

    private readonly IHttpContextAccessor _httpContextAccessor;


    public UpdateChannelGroupRequestHandler(IHttpContextAccessor httpContextAccessor, ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ChannelGroupDto?> Handle(UpdateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        //IQueryable<VideoStream> videoStreamsRepo = Repository.VideoStream.GetAllVideoStreams();

        //IQueryable<string> originalStreamsIds = videoStreamsRepo.Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == request.GroupName.ToLower()).Select(a => a.Id);

        string url = _httpContextAccessor.GetUrl();
        (ChannelGroupDto? cg, List<VideoStreamDto>? distinctList, List<StreamGroupDto>? streamGroups) = await Repository.ChannelGroup.UpdateChannelGroup(request, url, cancellationToken).ConfigureAwait(false);

        if (distinctList != null && distinctList.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);
        }

        if (streamGroups != null && streamGroups.Any())
        {
            await Publisher.Publish(new StreamGroupUpdateEvent(), cancellationToken).ConfigureAwait(false);
            //foreach (StreamGroupDto? streamGroup in streamGroups.Where(a => a is not null))
            //{
            //    await Publisher.Publish(new StreamGroupUpdateEvent(), cancellationToken).ConfigureAwait(false);
            //    //var streamGroup = await _context.GetStreamGroupDto(ret.Id, url, cancellationToken).ConfigureAwait(false);
            //    //if (streamGroup is not null && streamGroup.ChildVideoStreams.Any())
            //    //{
            //    //    await _publisher.Publish(new UpdateVideoStreamsEvent(streamGroup.ChildVideoStreams), cancellationToken).ConfigureAwait(false);
            //    //}
            //}
        }

        //if (originalStreamsIds.Any())
        //{

        //    //IQueryable<VideoStream> orginalStreams = Repository.VideoStream.GetVideoStreamsByMatchingIds(originalStreamsIds);
        //    //List<VideoStreamDto> originalStreamsDto = Mapper.Map<List<VideoStreamDto>>(orginalStreams);
        //    await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);
        //}

        if (cg is not null)
        {
            await Publisher.Publish(new UpdateChannelGroupEvent(), cancellationToken).ConfigureAwait(false);
        }

        return cg;
    }
}
