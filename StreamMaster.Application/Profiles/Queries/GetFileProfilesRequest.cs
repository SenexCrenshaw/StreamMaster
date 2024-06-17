namespace StreamMaster.Application.Profiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetFileProfilesRequest : IRequest<DataResponse<List<FileOutputProfileDto>>>;

internal class GetFileProfilesRequestHandler(IOptionsMonitor<FileOutputProfiles> intprofilesettings)
    : IRequestHandler<GetFileProfilesRequest, DataResponse<List<FileOutputProfileDto>>>
{
    public async Task<DataResponse<List<FileOutputProfileDto>>> Handle(GetFileProfilesRequest request, CancellationToken cancellationToken)
    {

        List<FileOutputProfileDto> ret = [];

        foreach (string key in intprofilesettings.CurrentValue.FileProfiles.Keys)
        {
            ret.Add(new FileOutputProfileDto
            {
                Name = key,
                IsReadOnly = intprofilesettings.CurrentValue.FileProfiles[key].IsReadOnly,
                EPGOutputProfile = intprofilesettings.CurrentValue.FileProfiles[key].EPGOutputProfile,
                M3UOutputProfile = intprofilesettings.CurrentValue.FileProfiles[key].M3UOutputProfile,
            });
        }

        return DataResponse<List<FileOutputProfileDto>>.Success(ret);
    }
}