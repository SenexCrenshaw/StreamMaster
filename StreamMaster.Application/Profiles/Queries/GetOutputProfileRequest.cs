namespace StreamMaster.Application.Profiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetOutputProfileRequest(string OutputProfileName) : IRequest<DataResponse<OutputProfileDto>>;

internal class GetOutputProfileRequestHandler(IOptionsMonitor<OutputProfileDict> intOutputProfileDict)
    : IRequestHandler<GetOutputProfileRequest, DataResponse<OutputProfileDto>>
{
    public async Task<DataResponse<OutputProfileDto>> Handle(GetOutputProfileRequest request, CancellationToken cancellationToken)
    {
        OutputProfileDto a = intOutputProfileDict.CurrentValue.GetProfileDto(request.OutputProfileName);
        return DataResponse<OutputProfileDto>.Success(a);
    }
}