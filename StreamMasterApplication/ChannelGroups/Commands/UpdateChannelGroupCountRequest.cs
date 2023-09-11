using FluentValidation;

using Microsoft.EntityFrameworkCore;

namespace StreamMasterApplication.ChannelGroups.Commands;
public record UpdateChannelGroupCountRequest(ChannelGroupDto ChannelGroupDto) : IRequest<bool> { }

public class UpdateChannelGroupCountRequestValidator : AbstractValidator<UpdateChannelGroupCountRequest>
{
    public UpdateChannelGroupCountRequestValidator()
    {
        _ = RuleFor(v => v.ChannelGroupDto).NotNull();
    }
}


[LogExecutionTimeAspect]
public class UpdateChannelGroupCountRequestHandler(ILogger<UpdateChannelGroupCountRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMemoryRequestHandler(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateChannelGroupCountRequest, bool>
{
    public async Task<bool> Handle(UpdateChannelGroupCountRequest request, CancellationToken cancellationToken)
    {
        if (request.ChannelGroupDto == null)
        {
            return false;
        }

        ChannelGroupStreamCount response = new();
        IQueryable<VideoStream> allVideoStreamsQuery = Repository.VideoStream.GetAllVideoStreams();
        List<VideoStream> videoStreamsForGroup = await allVideoStreamsQuery.Where(vs => vs.User_Tvg_group == request.ChannelGroupDto.Name).ToListAsync(cancellationToken: cancellationToken);
        HashSet<string> ids = new(videoStreamsForGroup.Select(a => a.Id));

        int hiddenCount = videoStreamsForGroup.Count(a => a.IsHidden);
        bool changed = false;

        if (request.ChannelGroupDto.TotalCount != ids.Count)
        {
            request.ChannelGroupDto.TotalCount = ids.Count;
            changed = true;
        }

        if (request.ChannelGroupDto.ActiveCount != ids.Count - hiddenCount)
        {
            request.ChannelGroupDto.ActiveCount = ids.Count - hiddenCount;
            changed = true;
        }

        if (request.ChannelGroupDto.HiddenCount != hiddenCount)
        {
            request.ChannelGroupDto.HiddenCount = hiddenCount;
            changed = true;
        }

        response.Id = request.ChannelGroupDto.Id;

        if (changed)
        {
            MemoryCache.AddOrUpdateChannelGroupVideoStreamCount(response);
        }
        return changed;
    }

}
