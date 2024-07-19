using StreamMaster.Streams.Domain.Extensions;

namespace StreamMaster.Application.Profiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetCommandProfilesRequest : IRequest<DataResponse<List<CommandProfileDto>>>;

internal class GetCommandProfilesRequestHandler(IOptionsMonitor<CommandProfileList> intprofilesettings)
    : IRequestHandler<GetCommandProfilesRequest, DataResponse<List<CommandProfileDto>>>
{
    public async Task<DataResponse<List<CommandProfileDto>>> Handle(GetCommandProfilesRequest request, CancellationToken cancellationToken)
    {

        List<CommandProfileDto> ret = [];

        foreach (string key in intprofilesettings.CurrentValue.CommandProfiles.Keys)
        {
            //ret.Add(new CommandProfileDto
            //{
            //    ProfileName = key,
            //    IsReadOnly = intprofilesettings.CurrentValue.CommandProfiles[key].IsReadOnly,
            //    Parameters = intprofilesettings.CurrentValue.CommandProfiles[key].Parameters,
            //    //Timeout = intprofilesettings.CurrentValue.CommandProfiles[key].Timeout,
            //    //IsM3U8 = intprofilesettings.CurrentValue.CommandProfiles[key].IsM3U8
            //});
            ret.Add(intprofilesettings.CurrentValue.CommandProfiles[key].ToCommandProfileDto(key));
        }

        return DataResponse<List<CommandProfileDto>>.Success(ret);
    }
}