namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetAvailableCountriesRequest : IRequest<DataResponse<List<CountryData>?>>;

internal class GetAvailableCountriesRequestHandler(ISchedulesDirectAPIService schedulesDirectAPIService) : IRequestHandler<GetAvailableCountriesRequest, DataResponse<List<CountryData>?>>
{
    public async Task<DataResponse<List<CountryData>?>> Handle(GetAvailableCountriesRequest request, CancellationToken cancellationToken)
    {
        List<CountryData>? countries = await schedulesDirectAPIService.GetAvailableCountriesAsync(cancellationToken);

        return DataResponse<List<CountryData>?>.Success(countries);
    }
}
