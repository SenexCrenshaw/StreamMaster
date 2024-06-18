namespace StreamMaster.Application.Profiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetVideoProfilesRequest : IRequest<DataResponse<List<VideoOutputProfileDto>>>;

internal class GetVideoProfilesRequestHandler(IOptionsMonitor<VideoOutputProfiles> intprofilesettings)
    : IRequestHandler<GetVideoProfilesRequest, DataResponse<List<VideoOutputProfileDto>>>
{
    public async Task<DataResponse<List<VideoOutputProfileDto>>> Handle(GetVideoProfilesRequest request, CancellationToken cancellationToken)
    {

        List<VideoOutputProfileDto> ret = [];

        foreach (string key in intprofilesettings.CurrentValue.VideoProfiles.Keys)
        {
            ret.Add(new VideoOutputProfileDto
            {
                ProfileName = key,
                IsReadOnly = intprofilesettings.CurrentValue.VideoProfiles[key].IsReadOnly,
                Parameters = intprofilesettings.CurrentValue.VideoProfiles[key].Parameters,
                Timeout = intprofilesettings.CurrentValue.VideoProfiles[key].Timeout,
                IsM3U8 = intprofilesettings.CurrentValue.VideoProfiles[key].IsM3U8
            });
        }

        return DataResponse<List<VideoOutputProfileDto>>.Success(ret);
    }
}