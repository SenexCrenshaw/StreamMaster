namespace StreamMaster.Application.EPGFiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetEPGNextEPGNumberRequest() : IRequest<DataResponse<int>>;

internal class GetEPGNextEPGNumberHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetEPGNextEPGNumberRequest, DataResponse<int>>
{
    public async Task<DataResponse<int>> Handle(GetEPGNextEPGNumberRequest request, CancellationToken cancellationToken = default)
    {
        int nextAvailableNumber = await Repository.EPGFile.GetNextAvailableEPGNumberAsync(cancellationToken).ConfigureAwait(false);

        return DataResponse<int>.Success(nextAvailableNumber);
    }
}
