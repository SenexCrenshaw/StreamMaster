using StreamMasterDomain.EnvironmentInfo;
using StreamMasterDomain.Extensions;

using System.Diagnostics;
using System.Text.Json;

namespace StreamMasterDomain.Configuration;

public interface IConfigFileProvider
{
    public Setting Setting { get; }

    void UpdateSetting(Setting setting);
}

public class ConfigFileProvider : IConfigFileProvider
{
    private readonly string _configFile;
    private IAppFolderInfo _appFolderInfo;
    private Setting currentSetting;

    public ConfigFileProvider(IAppFolderInfo appFolderInfo)
    {
        _appFolderInfo = appFolderInfo;
        _configFile = appFolderInfo.SettingFile;
        EnsureDefaultConfigFile();
        LoadSettings();

        if (currentSetting.ApiKey.IsNullOrWhiteSpace())
        {
            currentSetting.ApiKey = GenerateApiKey();
            UpdateSetting();
        }

        if (Debugger.IsAttached)
        {
            currentSetting.UiFolder = "devwwwroot";
            currentSetting.ApiKey = "f835904d5a2343d8ac567c026d6c08b2";
        }
    }

    public Setting Setting => currentSetting;

    public Setting GetSetting()
    {
        return currentSetting;
    }

    public void UpdateSetting(Setting? setting = null)
    {
        if (!Directory.Exists(_appFolderInfo.AppDataFolder))
        {
            _ = Directory.CreateDirectory(_appFolderInfo.AppDataFolder);
        }

        if (setting == null)
        {
            if (currentSetting == null)
            {
                currentSetting = new Setting();
            }

            setting = currentSetting;
        }

        string jsonString = JsonSerializer.Serialize(setting, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configFile, jsonString);
        currentSetting = setting;
    }

    private void EnsureDefaultConfigFile()
    {
        if (!File.Exists(_configFile))
        {
            UpdateSetting(new Setting());
        }
    }

    private string GenerateApiKey()
    {
        return Guid.NewGuid().ToString().Replace("-", "");
    }

    private void LoadSettings()
    {
        string jsonString;

        if (File.Exists(_configFile))
        {
            jsonString = File.ReadAllText(_configFile);
            var ret = JsonSerializer.Deserialize<Setting>(jsonString);
            if (ret != null)
            {
                currentSetting = ret;
                return;
            }
        }

        currentSetting = new Setting();
        UpdateSetting(currentSetting);
    }
}
