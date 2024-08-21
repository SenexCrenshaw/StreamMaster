namespace StreamMaster.Application.Profiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetOutputProfilesRequest : IRequest<DataResponse<List<OutputProfileDto>>>;

internal class GetOutputProfilesRequestHandler(IOptionsMonitor<OutputProfileDict> intOutPutProfileSettings)
    : IRequestHandler<GetOutputProfilesRequest, DataResponse<List<OutputProfileDto>>>
{
    public async Task<DataResponse<List<OutputProfileDto>>> Handle(GetOutputProfilesRequest request, CancellationToken cancellationToken)
    {
        List<OutputProfileDto> a = intOutPutProfileSettings.CurrentValue.GetProfilesDto();
        return await Task.FromResult(DataResponse<List<OutputProfileDto>>.Success(intOutPutProfileSettings.CurrentValue.GetProfilesDto()));
    }
}