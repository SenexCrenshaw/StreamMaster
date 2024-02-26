namespace StreamMaster.Application.Profiles.Queries;

public record GetFFMPEGProfiles : IRequest<FFMPEGProfileDtos>;

internal class GetFFMPEGProfilesHandler(IOptionsMonitor<FFMPEGProfiles> intprofilesettings) : IRequestHandler<GetFFMPEGProfiles, FFMPEGProfileDtos>
{
    public async Task<FFMPEGProfileDtos> Handle(GetFFMPEGProfiles request, CancellationToken cancellationToken)
    {

        FFMPEGProfileDtos ret = [];

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