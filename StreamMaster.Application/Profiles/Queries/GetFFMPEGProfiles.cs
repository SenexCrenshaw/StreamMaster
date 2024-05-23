namespace StreamMaster.Application.Profiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetFFMPEGProfiles : IRequest<List<FFMPEGProfileDto>>;

internal class GetFFMPEGProfilesHandler(IOptionsMonitor<FFMPEGProfiles> intprofilesettings) : IRequestHandler<GetFFMPEGProfiles, List<FFMPEGProfileDto>>
{
    public async Task<List<FFMPEGProfileDto>> Handle(GetFFMPEGProfiles request, CancellationToken cancellationToken)
    {

        List<FFMPEGProfileDto> ret = [];

        foreach (string key in intprofilesettings.CurrentValue.Profiles.Keys)
        {
            ret.Add(new FFMPEGProfileDto
            {
                Name = key,
                Parameters = intprofilesettings.CurrentValue.Profiles[key].Parameters,
                Timeout = intprofilesettings.CurrentValue.Profiles[key].Timeout,
                IsM3U8 = intprofilesettings.CurrentValue.Profiles[key].IsM3U8
            });
        }

        return ret;
    }
}