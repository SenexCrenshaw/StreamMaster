namespace StreamMaster.Application.Logos.CommandsOld;

public record AutoMatchLogoToStreamsRequest(List<string> Ids) : IRequest<LogoFileDto?>;

public class AutoMatchLogoToStreamsRequestHandler(IRepositoryWrapper Repository)
    : IRequestHandler<AutoMatchLogoToStreamsRequest, LogoFileDto?>
{
    public async Task<LogoFileDto?> Handle(AutoMatchLogoToStreamsRequest request, CancellationToken cancellationToken)
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