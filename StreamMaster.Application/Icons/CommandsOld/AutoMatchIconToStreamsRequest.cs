using FluentValidation;

namespace StreamMaster.Application.Icons.CommandsOld;

public record AutoMatchIconToStreamsRequest(List<string> Ids) : IRequest<LogoFileDto?>;

public class AutoMatchIconToStreamsRequestValidator : AbstractValidator<AutoMatchIconToStreamsRequest>
{
    public AutoMatchIconToStreamsRequestValidator()
    {
        _ = RuleFor(v => v.Ids).NotNull().NotEmpty();
    }
}

public class AutoMatchIconToStreamsRequestHandler(IRepositoryWrapper Repository)
    : IRequestHandler<AutoMatchIconToStreamsRequest, LogoFileDto?>
{
    public async Task<LogoFileDto?> Handle(AutoMatchIconToStreamsRequest request, CancellationToken cancellationToken)
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