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
            if (intprofilesettings.CurrentValue.OutProfiles.TryGetValue(key, out var profile))
            {
                ret.Add(new OutputProfileDto
                {
                    ProfileName = key,
                    IsReadOnly = profile.IsReadOnly,
                    EnableIcon = profile.EnableIcon,
                    EnableId = profile.EnableId,
                    EnableGroupTitle = profile.EnableGroupTitle,
                    EnableChannelNumber = profile.EnableChannelNumber,
                    Name = profile.Name,
                    EPGId = profile.EPGId,
                    Group = profile.Group,
                    //ChannelNumber = profile.ChannelNumber,
                });
            }


        }

        return DataResponse<List<OutputProfileDto>>.Success(ret);
    }
}