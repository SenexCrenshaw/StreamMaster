using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

using StreamMasterApplication.Common.Extensions;

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
    public int VideoStreamId { get; set; }
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
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public UpdateStreamGroupRequestHandler(
        IMapper mapper,
         IPublisher publisher,
         IHttpContextAccessor httpContextAccessor,
        IAppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
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
        string url = _httpContextAccessor.GetUrl();
        var ret = await _context.UpdateStreamGroupAsync(request, url, cancellationToken).ConfigureAwait(false);
        if (ret is not null)
        {
            await _publisher.Publish(new StreamGroupUpdateEvent(ret), cancellationToken).ConfigureAwait(false);
        }

        return ret;
    }
}
