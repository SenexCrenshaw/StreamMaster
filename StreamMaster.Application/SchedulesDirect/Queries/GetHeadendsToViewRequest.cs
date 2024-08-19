namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetHeadendsToViewRequest() : IRequest<DataResponse<List<HeadendToView>>>;

internal class GetHeadendsToViewRequestHandler(IOptionsMonitor<SDSettings> intSDSettings)
    : IRequestHandler<GetHeadendsToViewRequest, DataResponse<List<HeadendToView>>>
{
    private readonly SDSettings sdSettings = intSDSettings.CurrentValue;

    public async Task<DataResponse<List<HeadendToView>>> Handle(GetHeadendsToViewRequest request, CancellationToken cancellationToken)
    {

        return DataResponse<List<HeadendToView>>.Success(sdSettings.HeadendsToView);
    }
}
