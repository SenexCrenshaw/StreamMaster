namespace StreamMaster.Application.Profiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetOutputProfileRequest(string OutputProfileName) : IRequest<DataResponse<OutputProfileDto>>;

internal class GetOutputProfileRequestHandler(IOptionsMonitor<OutputProfiles> intprofilesettings)
    : IRequestHandler<GetOutputProfileRequest, DataResponse<OutputProfileDto>>
{
    public async Task<DataResponse<OutputProfileDto>> Handle(GetOutputProfileRequest request, CancellationToken cancellationToken)
    {
        if (intprofilesettings.CurrentValue.OutProfiles.TryGetValue(request.OutputProfileName, out var profile))
        {
            OutputProfileDto ret = new OutputProfileDto
            {
                ProfileName = request.OutputProfileName,
                IsReadOnly = profile.IsReadOnly,
                EnableIcon = profile.EnableIcon,
                EnableId = profile.EnableId,
                EnableGroupTitle = profile.EnableGroupTitle,
                EnableChannelNumber = profile.EnableChannelNumber,
                Name = profile.Name,
                EPGId = profile.EPGId,
                Group = profile.Group,
                //ChannelNumber = profile.ChannelNumber,
            };
            return DataResponse<OutputProfileDto>.Success(ret);
        }

        return DataResponse<OutputProfileDto>.ErrorWithMessage("Output Profile not found");
    }
}