using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Commands;

[RequireAll]
public record AddChannelGroupRequest(string GroupName, int Rank, string? Regex) : IRequest<ChannelGroupDto?>
{
}

public class AddChannelGroupRequestValidator : AbstractValidator<AddChannelGroupRequest>
{
    public AddChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.GroupName).NotNull().NotEmpty();
        _ = RuleFor(v => v.Rank).NotNull().GreaterThan(0);
    }
}

public class AddChannelGroupRequestHandler : IRequestHandler<AddChannelGroupRequest, ChannelGroupDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;
    private readonly ISender _sender;

    public AddChannelGroupRequestHandler(
        IMapper mapper,
        ISender sender,
         IPublisher publisher,
        IAppDbContext context
        )
    {
        _sender = sender;
        _publisher = publisher;
        _mapper = mapper;
        _context = context;
    }

    public async Task<ChannelGroupDto?> Handle(AddChannelGroupRequest request, CancellationToken cancellationToken)
    {
        if (await _context.ChannelGroups.AnyAsync(a => a.Name.ToLower() == request.GroupName.ToLower(), cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        ChannelGroup channelGroup = new() { Name = request.GroupName, Rank = request.Rank, IsReadOnly = false };
        if (!string.IsNullOrEmpty(request.Regex))
        {
            channelGroup.RegexMatch = request.Regex;
        }
        _ = _context.ChannelGroups.Add(channelGroup);
        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        ChannelGroupDto result = _mapper.Map<ChannelGroupDto>(channelGroup);
        await _publisher.Publish(new AddChannelGroupEvent(result), cancellationToken).ConfigureAwait(false);
        return result;
    }
}
