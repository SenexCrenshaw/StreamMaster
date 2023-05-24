using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups.Commands;

public record UpdateStreamGroupRequest(int StreamGroupId, string? Name, int? StreamGroupNumber, List<int> VideoStreamIds) : IRequest<StreamGroupDto?>
{
}

public class UpdateStreamGroupRequestValidator : AbstractValidator<UpdateStreamGroupRequest>
{
    public UpdateStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
           .NotNull()
           .GreaterThanOrEqualTo(0);
    }
}

public class UpdateStreamGroupRequestHandler : IRequestHandler<UpdateStreamGroupRequest, StreamGroupDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public UpdateStreamGroupRequestHandler(
        IMapper mapper,
         IPublisher publisher,
        IAppDbContext context)
    {
        _publisher = publisher;
        _mapper = mapper;
        _context = context;
    }

    public async Task<StreamGroupDto?> Handle(UpdateStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId< 1)
        {
            return null;
        }

        try
        {
            StreamGroup? streamGroup = await _context.StreamGroups.Include(a => a.VideoStreams).FirstOrDefaultAsync(a => a.Id == request.StreamGroupId, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (streamGroup == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                streamGroup.Name = request.Name;
            }

            if (request.StreamGroupNumber != null)
            {
                if (!await _context.StreamGroups.AnyAsync(a => a.StreamGroupNumber == (int)request.StreamGroupNumber, cancellationToken: cancellationToken).ConfigureAwait(false))
                {
                    streamGroup.StreamGroupNumber = (int)request.StreamGroupNumber;
                }
            }

            if (request.VideoStreamIds != null)
            {
                if (streamGroup.VideoStreams == null)
                {
                    streamGroup.VideoStreams = new List<VideoStream>();
                    _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    if (streamGroup.VideoStreams.Any())
                    {
                        streamGroup.VideoStreams.Clear();
                        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    }
                }

                foreach (var videoStreamId in request.VideoStreamIds)
                {
                    var stream = _context.VideoStreams.FirstOrDefault(a => a.Id == videoStreamId);
                    if (stream == null)
                    {
                        continue;
                    }

                    streamGroup.VideoStreams.Add(stream);
                }
            }

            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            StreamGroupDto ret = _mapper.Map<StreamGroupDto>(streamGroup);
            await _publisher.Publish(new StreamGroupUpdateEvent(ret), cancellationToken).ConfigureAwait(false);
            return ret;
        }
        catch (Exception)
        {
        }

        return null;
    }
}
