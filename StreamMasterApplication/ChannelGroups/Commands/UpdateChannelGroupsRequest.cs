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
public record UpdateChannelGroupsRequest(IEnumerable<UpdateChannelGroupRequest> ChannelGroupRequests) : IRequest<IEnumerable<ChannelGroupDto>?>
{
}

public class UpdateChannelGroupsRequestValidator : AbstractValidator<UpdateChannelGroupsRequest>
{
    //public UpdateChannelGroupsRequestValidator()
    //{
    //    _ = RuleFor(v => v.GroupName).NotNull().NotEmpty();
    //    _ = RuleFor(v => v.Rank).NotNull().GreaterThan(0);
    //}
}

public class UpdateChannelGroupsRequestHandler : IRequestHandler<UpdateChannelGroupsRequest, IEnumerable<ChannelGroupDto>?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public UpdateChannelGroupsRequestHandler(
         IMapper mapper,
           IPublisher publisher,
        IAppDbContext context
        )
    {
        _publisher = publisher;
        _mapper = mapper;
        _context = context;
    }

    public async Task<IEnumerable<ChannelGroupDto>?> Handle(UpdateChannelGroupsRequest requests, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = new();
        List<ChannelGroupDto> cgResults = new();

        foreach (UpdateChannelGroupRequest request in requests.ChannelGroupRequests)
        {
            ChannelGroup? channelGroup = await _context.ChannelGroups.FirstOrDefaultAsync(a => a.Name.ToLower() == request.GroupName.ToLower(), cancellationToken: cancellationToken).ConfigureAwait(false);

            if (channelGroup == null)
            {
                continue;
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

            _ = _context.ChannelGroups.Update(channelGroup);
            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            cgResults.Add(_mapper.Map<ChannelGroupDto>(channelGroup));

            if (isChanged)
            {
                results.AddRange(_context.VideoStreams
                    .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
                    .AsNoTracking()
                    .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider));
            }
        }

        await _publisher.Publish(new UpdateChannelGroupsEvent(cgResults), cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await _publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }

        return cgResults;
    }
}
