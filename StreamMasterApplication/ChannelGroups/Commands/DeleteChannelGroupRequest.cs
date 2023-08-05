using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Repository;

namespace StreamMasterApplication.ChannelGroups.Commands;

[RequireAll]
public record DeleteChannelGroupRequest(string GroupName) : IRequest<int?>
{
}

public class DeleteChannelGroupRequestValidator : AbstractValidator<DeleteChannelGroupRequest>
{
    public DeleteChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.GroupName).NotNull().NotEmpty();
    }
}

public class DeleteChannelGroupRequestHandler : IRequestHandler<DeleteChannelGroupRequest, int?>
{
    private readonly IAppDbContext _context;

    public DeleteChannelGroupRequestHandler(

        IAppDbContext context
        )
    {
        _context = context;
    }

    public async Task<int?> Handle(DeleteChannelGroupRequest request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await _context.ChannelGroups.FirstOrDefaultAsync(a => a.Name.ToLower() == request.GroupName.ToLower(), cancellationToken: cancellationToken).ConfigureAwait(false);

        if (channelGroup == null)
        {
            return null;
        }
        channelGroup.AddDomainEvent(new DeleteChannelGroupEvent(channelGroup.Id));

        _ = _context.ChannelGroups.Remove(channelGroup);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return channelGroup.Id;
    }
}
