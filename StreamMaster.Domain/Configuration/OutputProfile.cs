using AutoMapper.Configuration.Annotations;

using MessagePack;

using System.Text.Json.Serialization;

namespace StreamMaster.Domain.Configuration;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class OutputProfile
{
    [Ignore]
    [JsonIgnore]
    [IgnoreMember]

    public static string APIName => "GetOutputProfiles";

    public bool IsReadOnly { get; set; } = false;
    public bool EnableIcon { get; set; } = true;
    //public bool EnableId { get; set; } = true;
    public bool EnableGroupTitle { get; set; } = true;

    public bool EnableChannelNumber { get; set; } = true;

    //public bool AppendChannelNumberToId { get; set; } = false;

    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    //public string EPGId { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    //public string ChannelNumber { get; set; } = string.Empty;

}

//[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record OutputProfileRequest
{
    public bool? EnableIcon { get; set; }
    public bool? EnableGroupTitle { get; set; }
    //public bool? EnableId { get; set; }
    public bool? EnableChannelNumber { get; set; }
    //public bool? AppendChannelNumberToId { get; set; }
    public string? Id { get; set; }
    public string? Name { get; set; }
    //public string? EPGId { get; set; }
    public string? Group { get; set; }
    //public string? ChannelNumber { get; set; }

}

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class OutputProfileDto : OutputProfile
{
    public string ProfileName { get; set; } = "";
}

public class OutputProfileDict : IProfileDict<OutputProfile>
{
    [JsonPropertyName("OutputProfiles")]
    public Dictionary<string, OutputProfile> OutputProfiles { get; set; } = [];
    public OutputProfile? GetProfile(string OutputProfileName)
    {
        return Profiles.TryGetValue(OutputProfileName, out OutputProfile? existingProfile)
            ? existingProfile
            : null;
    }

    [JsonIgnore]
    public Dictionary<string, OutputProfile> Profiles => OutputProfiles;

    public OutputProfileDto GetDefaultProfileDto(string defaultName = "Default")
    {

        OutputProfile? defaultProfile = GetProfile(defaultName);
        return defaultProfile == null
            ? throw new Exception($"Command Profile {defaultName} not found")
            : GetProfileDtoFromProfile(defaultProfile, defaultName);
    }

    public OutputProfileDto GetProfileDtoFromProfile(OutputProfile outputProfile, string ProfileName)
    {
        return new OutputProfileDto
        {
            ProfileName = ProfileName,
            IsReadOnly = outputProfile.IsReadOnly,
            EnableIcon = outputProfile.EnableIcon,
            //EnableId = outputProfile.EnableId,
            Id = outputProfile.Id,
            EnableGroupTitle = outputProfile.EnableGroupTitle,
            EnableChannelNumber = outputProfile.EnableChannelNumber,
            Name = outputProfile.Name,
            //EPGId = outputProfile.EPGId,
            Group = outputProfile.Group,
            //AppendChannelNumberToId = outputProfile.AppendChannelNumberToId,
        };
    }

    public OutputProfileDto GetProfileDto(string OutputProfileName)
    {
        return GetDefaultProfileDto(OutputProfileName);

    }
    public List<OutputProfileDto> GetProfilesDto()
    {
        List<OutputProfileDto> ret = [];

        foreach (string key in Profiles.Keys)
        {
            if (Profiles.TryGetValue(key, out OutputProfile? profile))
            {
                ret.Add(GetProfileDtoFromProfile(profile, key));
            }
        }
        return ret;
    }

    public List<OutputProfile> GetProfiles()
    {
        List<OutputProfile> ret = [];

        foreach (string key in Profiles.Keys)
        {
            if (Profiles.TryGetValue(key, out OutputProfile? profile))
            {
                ret.Add(profile);
            }
        }
        return ret;
    }


    public void AddProfile(string ProfileName, OutputProfile Profile)
    {
        OutputProfiles.Add(ProfileName, Profile);

    }
    public void AddProfiles(Dictionary<string, dynamic> profiles)
    {
        OutputProfiles = profiles.ToDictionary(
          kvp => kvp.Key,
          kvp => (OutputProfile)kvp.Value
      );
    }

    public void RemoveProfile(string ProfileName)
    {
        OutputProfiles.Remove(ProfileName);
    }
}