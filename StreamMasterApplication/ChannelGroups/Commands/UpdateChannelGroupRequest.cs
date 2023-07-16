using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

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
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public UpdateChannelGroupRequestHandler(
         IMapper mapper,
           IPublisher publisher,
        IAppDbContext context
        )
    {
        _publisher = publisher;
        _mapper = mapper;
        _context = context;
    }

    public async Task<ChannelGroupDto?> Handle(UpdateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await _context.ChannelGroups.FirstOrDefaultAsync(a => a.Name.ToLower() == request.GroupName.ToLower(), cancellationToken: cancellationToken).ConfigureAwait(false);

        if (channelGroup == null)
        {
            return null;
        }

        if (request.Rank != null)
        {
            channelGroup.Rank = (int)request.Rank;
        }

        bool isChanged = false;

        if (request.IsHidden != null)
        {
            channelGroup.IsHidden = (bool)request.IsHidden;

            await _context.VideoStreams
                .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsHidden, (bool)request.IsHidden), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            isChanged = true;
        }

        if (!string.IsNullOrEmpty(request.NewGroupName))
        {
            await _context.VideoStreams
               .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
               .ExecuteUpdateAsync(s => s.SetProperty(b => b.User_Tvg_group, request.NewGroupName), cancellationToken: cancellationToken)
               .ConfigureAwait(false);

            channelGroup.Name = request.NewGroupName;
            isChanged = true;
        }

        if (!string.IsNullOrEmpty(request.Regex))
        {
            channelGroup.RegexMatch = request.Regex;
        }

        _context.ChannelGroups.Update(channelGroup);
         await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        ChannelGroupDto cgresult = _mapper.Map<ChannelGroupDto>(channelGroup);
        await _publisher.Publish(new UpdateChannelGroupEvent(cgresult), cancellationToken).ConfigureAwait(false);

        if (!isChanged)
        {
            return cgresult;
        }

        List<VideoStreamDto> results = _context.VideoStreams
           .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
           .AsNoTracking()
           .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider).ToList();

        await _publisher.Publish(new UpdateChannelGroupEvent(cgresult), cancellationToken).ConfigureAwait(false);
        await _publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
   
        return cgresult;
    }
}
