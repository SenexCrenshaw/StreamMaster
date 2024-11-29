using System.Text.Json.Serialization;

using AutoMapper.Configuration.Annotations;

using MessagePack;

using StreamMaster.Domain.Crypto;

namespace StreamMaster.Domain.Configuration;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class CustomLogo
{
    [Ignore]
    [JsonIgnore]
    [IgnoreMember]
    public static string APIName => "GetCustomLogos";
    public required string Name { get; set; }
    public bool IsReadOnly { get; set; }
    public required string Value { get; set; }
    [IgnoreMember]
    public int FileId { get; set; } = -1;
}

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class CustomLogoDto : CustomLogo
{
    public required string Source { get; set; }
}

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CustomLogoRequest
{
    public string Source { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class CustomLogoDict : IProfileDict<CustomLogo>
{
    [JsonPropertyName("CustomLogos")]
    public Dictionary<string, CustomLogo> CustomLogos { get; set; } = [];

    public CustomLogo? GetCustomLogo(string Source)
    {
        return CustomLogos.TryGetValue(Source, out CustomLogo? existingCustomLogo)
            ? existingCustomLogo
            : null;
    }



    [JsonIgnore]
    public Dictionary<string, CustomLogo> Profiles => CustomLogos;

    public static CustomLogoDto GetCustomLogoDtoFromCustomLogo(CustomLogo CustomLogo, string Source)
    {
        return new CustomLogoDto
        {
            Source = Source.FromUrlSafeBase64String(),
            Name = CustomLogo.Name,
            IsReadOnly = CustomLogo.IsReadOnly,
            Value = CustomLogo.Value.FromUrlSafeBase64String()
        };
    }

    public List<CustomLogoDto> GetCustomLogosDto()
    {
        List<CustomLogoDto> ret = [];

        foreach (string key in CustomLogos.Keys)
        {
            if (CustomLogos.TryGetValue(key, out CustomLogo? CustomLogo))
            {
                ret.Add(GetCustomLogoDtoFromCustomLogo(CustomLogo, key));
            }
        }
        return [.. ret.OrderBy(a => a.Name)];
    }

    public List<CustomLogo> GetCustomLogos()
    {
        return [.. CustomLogos.Values];
    }

    public CustomLogo? GetProfile(string OutputProfileName)
    {
        return CustomLogos.TryGetValue(OutputProfileName, out CustomLogo? existingProfile)
            ? existingProfile
            : null;
    }

    public bool IsReadOnly(string ProfileName)
    {
        return GetProfile(ProfileName)?.IsReadOnly ?? false;
    }

    public void AddCustomLogo(string Url, string Name)
    {
        if (CustomLogos.ContainsKey(Url))
        {
            return;
        }

        CustomLogo customLogo = new()
        {
            Name = Name,
            Value = Url,
            IsReadOnly = false
        };
        CustomLogos.Add(Url, customLogo);
    }

    public void AddProfile(string ProfileName, CustomLogo Profile)
    {
        CustomLogos.Add(ProfileName, Profile);
    }

    public void AddProfiles(Dictionary<string, dynamic> profiles)
    {
        CustomLogos = profiles.ToDictionary(
         kvp => kvp.Key,
         kvp => (CustomLogo)kvp.Value
     );
    }

    public void RemoveProfile(string ProfileName)
    {
        CustomLogos.Remove(ProfileName.ToUrlSafeBase64String());
    }
}