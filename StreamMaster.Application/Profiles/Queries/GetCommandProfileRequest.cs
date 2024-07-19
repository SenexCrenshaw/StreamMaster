using StreamMaster.Streams.Domain.Extensions;

namespace StreamMaster.Application.Profiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetCommandProfileRequest(string CommandProfileName, int StreamGroupId, int StreamGroupProfileId) : IRequest<DataResponse<CommandProfileDto>>;

internal class GetCommandProfileRequestHandler(IOptionsMonitor<CommandProfileList> intProfileSettings, IRepositoryWrapper repositoryWrapper, IOptionsMonitor<Setting> intSettings)
    : IRequestHandler<GetCommandProfileRequest, DataResponse<CommandProfileDto>>
{
    public async Task<DataResponse<CommandProfileDto>> Handle(GetCommandProfileRequest request, CancellationToken cancellationToken)
    {
        CommandProfileDto ret = new();
        string profileName = BuildInfo.DefaultCommandProfileName;

        if (request.CommandProfileName == BuildInfo.DefaultCommandProfileName)
        {

            StreamGroupProfile? profile = repositoryWrapper.StreamGroupProfile.GetStreamGroupProfile(request.StreamGroupId, request.StreamGroupProfileId);

            if (profile != null && !string.IsNullOrEmpty(profile.CommandProfileName) && profile.CommandProfileName != BuildInfo.DefaultCommandProfileName)
            {
                profileName = profile.CommandProfileName;
            }
        }

        if (intProfileSettings.CurrentValue.CommandProfiles.TryGetValue(profileName, out CommandProfile? existingProfile))
        {
            ret = existingProfile.ToCommandProfileDto(profileName);
        }

        return DataResponse<CommandProfileDto>.Success(ret);
    }
}