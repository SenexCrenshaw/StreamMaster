namespace StreamMaster.Application.EPGFiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetEPGNextEPGNumberRequest() : IRequest<APIResponse<int>>;

internal class GetEPGNextEPGNumberHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetEPGNextEPGNumberRequest, APIResponse<int>>
{
    public async Task<APIResponse<int>> Handle(GetEPGNextEPGNumberRequest request, CancellationToken cancellationToken = default)
    {
        int nextAvailableNumber = await Repository.EPGFile.GetNextAvailableEPGNumberAsync(cancellationToken).ConfigureAwait(false);

        return APIResponse<int>.Success(nextAvailableNumber);
    }
}
