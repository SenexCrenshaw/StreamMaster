namespace StreamMaster.Application.EPGFiles.Queries;

public record GetEPGNextEPGNumber() : IRequest<int>;

internal class GetEPGNextEPGNumberHandler(ILogger<GetEPGNextEPGNumber> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetEPGNextEPGNumber, int>
{
    public async Task<int> Handle(GetEPGNextEPGNumber request, CancellationToken cancellationToken = default)
    {
        int nextAvailableNumber = await Repository.EPGFile.GetNextAvailableEPGNumberAsync(cancellationToken).ConfigureAwait(false);

        return nextAvailableNumber;
    }
}
