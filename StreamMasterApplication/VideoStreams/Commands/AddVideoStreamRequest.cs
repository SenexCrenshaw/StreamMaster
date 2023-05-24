using AutoMapper;

using FluentValidation;

using MediatR;

using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public record AddVideoStreamRequest(
     string Tvg_name,
    int? Tvg_chno,
    string? Tvg_group,
    string? Tvg_ID,
    string? Tvg_logo,
    string? Url,
    int? IPTVChannelHandler,
    bool? createChannel
    ) : IRequest<VideoStreamDto?>
{
}

public class AddVideoStreamRequestValidator : AbstractValidator<AddVideoStreamRequest>
{
    public AddVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.Tvg_name).NotNull().NotEmpty();
    }
}

public class AddVideoStreamRequestHandler : IRequestHandler<AddVideoStreamRequest, VideoStreamDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;
    private readonly ISender _sender;

    public AddVideoStreamRequestHandler(
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

    public async Task<VideoStreamDto?> Handle(AddVideoStreamRequest request, CancellationToken cancellationToken)
    {
        SettingDto settings = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);

        VideoStream VideoStream = new()
        {
            CUID = request.Tvg_name,
            IsUserCreated = true,

            Tvg_chno = request.Tvg_chno is null ? 0 : (int)request.Tvg_chno,
            User_Tvg_chno = request.Tvg_chno is null ? 0 : (int)request.Tvg_chno,

            Tvg_group = request.Tvg_group is null ? "All" : request.Tvg_group,
            User_Tvg_group = request.Tvg_group is null ? "All" : request.Tvg_group,

            Tvg_ID = request.Tvg_ID is null ? "dummy" : request.Tvg_ID,
            User_Tvg_ID = request.Tvg_ID is null ? "dummy" : request.Tvg_ID,

            Tvg_logo = request.Tvg_logo is null ? settings.StreamMasterIcon : request.Tvg_logo,
            User_Tvg_logo = request.Tvg_logo is null ? settings.StreamMasterIcon : request.Tvg_logo,

            Tvg_name = request.Tvg_name,
            User_Tvg_name = request.Tvg_name,

            Url = request.Url ?? string.Empty,
            User_Url = request.Url ?? string.Empty
        };

        _ = await _context.VideoStreams.AddAsync(VideoStream, cancellationToken).ConfigureAwait(false);

        if (await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0)
        {
            VideoStreamDto ret = _mapper.Map<VideoStreamDto>(VideoStream);

            await _publisher.Publish(new AddVideoStreamEvent(ret), cancellationToken).ConfigureAwait(false);
            return ret;
        }

        return null;
    }
}
