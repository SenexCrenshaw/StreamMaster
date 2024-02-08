using FluentValidation;

using StreamMaster.Application.VideoStreams.Events;
using StreamMaster.Domain.Requests;

namespace StreamMaster.Application.VideoStreams.Commands;

public class CreateVideoStreamRequestValidator : AbstractValidator<CreateVideoStreamRequest>
{
    public CreateVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.Tvg_name).NotNull().NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class CreateVideoStreamRequestHandler(ILogger<CreateVideoStreamRequest> logger, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher)
    : IRequestHandler<CreateVideoStreamRequest, VideoStreamDto?>
{
    public async Task<VideoStreamDto?> Handle(CreateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        VideoStream? stream = await Repository.VideoStream.CreateVideoStreamAsync(request, cancellationToken);

        if (stream != null)
        {
            VideoStreamDto streamDto = Mapper.Map<VideoStreamDto>(stream);

            await Publisher.Publish(new CreateVideoStreamEvent(streamDto), cancellationToken).ConfigureAwait(false);
            return streamDto;
        }

        return null;
    }
}