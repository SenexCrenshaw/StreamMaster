using FluentValidation;

namespace StreamMasterApplication.Icons.Commands;

public record AutoMatchIconToStreamsRequest(List<string> Ids) : IRequest<IconFileDto?> { }

public class AutoMatchIconToStreamsRequestValidator : AbstractValidator<AutoMatchIconToStreamsRequest>
{
    public AutoMatchIconToStreamsRequestValidator()
    {
        _ = RuleFor(v => v.Ids).NotNull().NotEmpty();
    }
}

public class AutoMatchIconToStreamsRequestHandler : BaseMediatorRequestHandler, IRequestHandler<AutoMatchIconToStreamsRequest, IconFileDto?>
{

    public AutoMatchIconToStreamsRequestHandler(ILogger<AutoMatchIconToStreamsRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }



    public async Task<IconFileDto?> Handle(AutoMatchIconToStreamsRequest request, CancellationToken cancellationToken)
    {
        if (request.Ids == null || request.Ids.Count == 0)
        {
            return null;
        }


        _ = await Repository.SaveAsync().ConfigureAwait(false);
        //if (videoStreamDtos.Any())
        //{
        //    await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);
        //}

        return null;
    }



    private class WeightedMatch
    {
        public required string Name { get; set; }
        public double Weight { get; set; }
    }
}