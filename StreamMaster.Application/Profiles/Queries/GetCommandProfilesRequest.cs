namespace StreamMaster.Application.Profiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetCommandProfilesRequest : IRequest<DataResponse<List<CommandProfileDto>>>;

internal class GetCommandProfilesRequestHandler(IProfileService profileService)
    : IRequestHandler<GetCommandProfilesRequest, DataResponse<List<CommandProfileDto>>>
{
    public async Task<DataResponse<List<CommandProfileDto>>> Handle(GetCommandProfilesRequest request, CancellationToken cancellationToken)
    {
        return DataResponse<List<CommandProfileDto>>.Success(profileService.GetCommandProfiles());
    }
}