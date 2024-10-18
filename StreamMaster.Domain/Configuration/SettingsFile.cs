namespace StreamMaster.Domain.Configuration;

public class SettingsFile<T>(string fileName, Type settingType)
{
    public string FileName { get; set; } = fileName;
    public Type SettingType { get; set; } = settingType;
}
