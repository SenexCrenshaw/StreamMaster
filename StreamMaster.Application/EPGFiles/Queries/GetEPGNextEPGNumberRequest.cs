namespace StreamMaster.Application.EPGFiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetEPGNextEPGNumberRequest() : IRequest<int>;

internal class GetEPGNextEPGNumberHandler(ILogger<GetEPGNextEPGNumberRequest> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetEPGNextEPGNumberRequest, int>
{
    public async Task<int> Handle(GetEPGNextEPGNumberRequest request, CancellationToken cancellationToken = default)
    {
        int nextAvailableNumber = await Repository.EPGFile.GetNextAvailableEPGNumberAsync(cancellationToken).ConfigureAwait(false);

        return nextAvailableNumber;
    }
}
