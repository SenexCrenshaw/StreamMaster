using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups.Commands;

public class UpdateStreamGroupRequestValidator : AbstractValidator<UpdateStreamGroupRequest>
{
    public UpdateStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
           .NotNull()
           .GreaterThanOrEqualTo(0);
    }
}

public class VideoStreamIsReadOnly
{
    public bool IsReadOnly { get; set; }
    public string VideoStreamId { get; set; }
}

public record UpdateStreamGroupRequest(
    int StreamGroupId,
    string? Name,
    int? StreamGroupNumber,
    List<VideoStreamIsReadOnly>? VideoStreams,
    List<string>? ChannelGroupNames
    ) : IRequest<StreamGroupDto?>
{
}

public class UpdateStreamGroupRequestHandler : IRequestHandler<UpdateStreamGroupRequest, StreamGroupDto?>
{
    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublisher _publisher;

    public UpdateStreamGroupRequestHandler(
         IPublisher publisher,
         IHttpContextAccessor httpContextAccessor,
        IAppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _publisher = publisher;
        _context = context;
    }

    public async Task<StreamGroupDto?> Handle(UpdateStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1)
        {
            return null;
        }
        string url = _httpContextAccessor.GetUrl();
        var streamGroup = await _context.UpdateStreamGroupAsync(request, url, cancellationToken).ConfigureAwait(false);
        if (streamGroup is not null)
        {
            //var streamGroup = await _context.GetStreamGroupDto(ret.Id, url, cancellationToken).ConfigureAwait(false);
            if (streamGroup is not null && streamGroup.ChildVideoStreams.Any())
            {
                await _publisher.Publish(new UpdateVideoStreamsEvent(streamGroup.ChildVideoStreams), cancellationToken).ConfigureAwait(false);
            }
            await _publisher.Publish(new StreamGroupUpdateEvent(streamGroup), cancellationToken).ConfigureAwait(false);
        }

        return streamGroup;
    }
}
