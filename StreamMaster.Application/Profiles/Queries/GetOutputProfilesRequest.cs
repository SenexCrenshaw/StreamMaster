namespace StreamMaster.Application.Profiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetOutputProfilesRequest : IRequest<DataResponse<List<OutputProfileDto>>>;

internal class GetOutputProfilesRequestHandler(IOptionsMonitor<OutputProfiles> intprofilesettings)
    : IRequestHandler<GetOutputProfilesRequest, DataResponse<List<OutputProfileDto>>>
{
    public async Task<DataResponse<List<OutputProfileDto>>> Handle(GetOutputProfilesRequest request, CancellationToken cancellationToken)
    {

        List<OutputProfileDto> ret = [];

        foreach (string key in intprofilesettings.CurrentValue.OutProfiles.Keys)
        {
            ret.Add(new OutputProfileDto
            {
                Name = key,
                IsReadOnly = intprofilesettings.CurrentValue.OutProfiles[key].IsReadOnly,
                EnableIcon = intprofilesettings.CurrentValue.OutProfiles[key].EnableIcon,
                TVGName = intprofilesettings.CurrentValue.OutProfiles[key].TVGName,
                ChannelId = intprofilesettings.CurrentValue.OutProfiles[key].ChannelId,
                TVGId = intprofilesettings.CurrentValue.OutProfiles[key].TVGId,
                TVGGroup = intprofilesettings.CurrentValue.OutProfiles[key].TVGGroup,
                ChannelNumber = intprofilesettings.CurrentValue.OutProfiles[key].ChannelNumber,
                GroupTitle = intprofilesettings.CurrentValue.OutProfiles[key].GroupTitle,
            });
        }

        return DataResponse<List<OutputProfileDto>>.Success(ret);
    }
}