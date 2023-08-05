using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository;

namespace StreamMasterApplication.ChannelGroups.Commands;

[RequireAll]
public record UpdateChannelGroupOrderRequest(List<ChannelGroupArg>? ChannelGroups) : IRequest<IEnumerable<ChannelGroupDto>?>
{
}

public class UpdateChannelGroupOrderRequestValidator : AbstractValidator<UpdateChannelGroupOrderRequest>
{
    public UpdateChannelGroupOrderRequestValidator()
    {
        _ = RuleFor(v => v.ChannelGroups).NotNull().NotEmpty();
    }
}

public class UpdateChannelGroupOrderRequestHandler : IRequestHandler<UpdateChannelGroupOrderRequest, IEnumerable<ChannelGroupDto>?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public UpdateChannelGroupOrderRequestHandler(
         IMapper mapper,
           IPublisher publisher,
        IAppDbContext context
        )
    {
        _publisher = publisher;
        _mapper = mapper;
        _context = context;
    }

    public async Task<IEnumerable<ChannelGroupDto>?> Handle(UpdateChannelGroupOrderRequest request, CancellationToken cancellationToken)
    {
        if (request.ChannelGroups == null || !request.ChannelGroups.Any())
        {
            return null;
        }

        _ = await _context.ChannelGroups.ExecuteDeleteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        List<ChannelGroup> cgs = _mapper.Map<List<ChannelGroup>>(request.ChannelGroups);
        await _context.ChannelGroups.AddRangeAsync(cgs, cancellationToken).ConfigureAwait(false);
        //foreach (var channelGroupDto in request.ChannelGroups)
        //{
        //    var channelGroup = new ChannelGroup
        //    {
        //        Name = channelGroupDto.Name,
        //        Rank = channelGroupDto.Rank,
        //        RegexMatch = channelGroupDto.RegexMatch ?? "",
        //        IsHidden = channelGroupDto.IsHidden ?? false,
        //        IsReadOnly = channelGroupDto.IsReadOnly ?? false,
        //    };
        //    _context.ChannelGroups.Add(channelGroup);
        //}

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await _publisher.Publish(new AddChannelGroupsEvent(), cancellationToken).ConfigureAwait(false);

        List<ChannelGroupDto> ret = await _context.ChannelGroups
           .AsNoTracking()
           .ProjectTo<ChannelGroupDto>(_mapper.ConfigurationProvider)
           .OrderBy(x => x.Rank)
           .ToListAsync(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
