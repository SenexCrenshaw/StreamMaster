using AutoMapper;

using FluentValidation;

using MediatR;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups.Commands;

[RequireAll]
public class AddStreamGroupRequest : IRequest<StreamGroupDto?>
{
    public AddStreamGroupRequest()
    {
        // StreamGroupVideoStreams = new();
    }

    public string Name { get; set; } = string.Empty;
    public int StreamGroupNumber { get; set; }
    //public List<StreamGroupVideoStream> StreamGroupVideoStreams { get; set; }
}

public class AddStreamGroupRequestValidator : AbstractValidator<AddStreamGroupRequest>
{
    public AddStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotEmpty()
            .GreaterThan(0);

        _ = RuleFor(v => v.Name)
           .MaximumLength(32)
           .NotEmpty();

        //_ = RuleFor(v => v.StreamGroupVideoStreams).NotNull().NotEmpty();
    }
}

public class AddStreamGroupRequestHandler : IRequestHandler<AddStreamGroupRequest, StreamGroupDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public AddStreamGroupRequestHandler(
        IMapper mapper,
        IPublisher publisher,
        IAppDbContext context
        )
    {
        _publisher = publisher;
        _mapper = mapper;
        _context = context;
    }

    public async Task<StreamGroupDto?> Handle(AddStreamGroupRequest command, CancellationToken cancellationToken)
    {
        if (command.StreamGroupNumber<1)
        {
            return null;
        }
            
        if (_context.StreamGroups.Any(a => a.StreamGroupNumber == command.StreamGroupNumber))
        {
            command.StreamGroupNumber = _context.StreamGroups.Max(a => a.StreamGroupNumber) + 1;
        }

        // List<StreamGroupVideoStream> sgt = _mapper.Map<List<StreamGroupVideoStream>>(command.StreamGroupVideoStreams);
        StreamGroup entity = new()
        {
            Name = command.Name,
            StreamGroupNumber = command.StreamGroupNumber,
            // StreamGroupVideoStreams = sgt,
        };

        _ = _context.StreamGroups.Add(entity);
        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        StreamGroupDto ret = _mapper.Map<StreamGroupDto>(entity);
        await _publisher.Publish(new StreamGroupUpdateEvent(ret), cancellationToken).ConfigureAwait(false);
        return ret;
    }
}
