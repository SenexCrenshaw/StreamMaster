using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups.Commands;

public record UpdateStreamGroupRequest(
    int StreamGroupId,
    string? Name,
    int? StreamGroupNumber,
    List<int>? VideoStreamIds,
    List<string>? ChannelGroupNames
    ) : IRequest<StreamGroupDto?>
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
        if (request.StreamGroupId < 1)
        {
            return null;
        }

        try
        {
            StreamGroup? streamGroup = await _context.StreamGroups
                .Include(a => a.VideoStreams)
                .Include(a => a.ChannelGroups)
                .FirstOrDefaultAsync(a => a.Id == request.StreamGroupId, cancellationToken: cancellationToken).ConfigureAwait(false);

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

            if (request.ChannelGroupNames != null)
            {
                if (streamGroup.ChannelGroups == null)
                {
                    streamGroup.ChannelGroups = new List<ChannelGroup>();
                    _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    if (streamGroup.ChannelGroups.Any())
                    {
                        streamGroup.ChannelGroups.Clear();
                        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    }
                }

                foreach (var channelGroupName in request.ChannelGroupNames)
                {
                    var channelGroup = _context.ChannelGroups.FirstOrDefault(a => a.Name == channelGroupName);
                    if (channelGroup == null)
                    {
                        continue;
                    }

                    streamGroup.ChannelGroups.Add(channelGroup);
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
            var existingIds = streamGroup.VideoStreams.Select(a => a.Id).ToList();

            foreach (var channegroup in streamGroup.ChannelGroups)
            {
                var streams = _context.VideoStreams
                    .Where(a => !existingIds.Contains(a.Id) && a.User_Tvg_group == channegroup.Name)
                    .AsNoTracking()
                    .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider)
                    .ToList();
                foreach (var stream in streams)
                {
                    stream.IsReadOnly = true;
                }
                ret.VideoStreams.AddRange(streams);
            }
            await _publisher.Publish(new StreamGroupUpdateEvent(ret), cancellationToken).ConfigureAwait(false);
            return ret;
        }
        catch (Exception)
        {
        }

        return null;
    }
}
