using StreamMaster.Domain.Configuration;

using System.Text.Json;

namespace StreamMaster.Domain.Helpers;

public static class SettingsHelper
{
    public static readonly object FileLock = new();
    public static T? GetSetting<T>(string SettingsFile)
    {
        lock (FileLock)
        {
            if (!File.Exists(SettingsFile))
            {
                return default;
            }

            string jsonString;
            T? ret;

            try
            {
                jsonString = File.ReadAllText(SettingsFile);
                ret = JsonSerializer.Deserialize<T>(jsonString);
                return ret != null ? ret : default;
            }
            catch (Exception ex)
            {
                return default;
            }
        }
    }

    public static void UpdateSetting(dynamic setting)
    {
        string fileName = "";
        string? dir = null;

        if (typeof(Setting).IsAssignableFrom(setting.GetType()))
        {
            fileName = BuildInfo.SettingsFile;
            dir = Path.GetDirectoryName(fileName);
        }

        if (typeof(CommandProfileDict).IsAssignableFrom(setting.GetType()))
        {
            fileName = BuildInfo.CommandProfileSettingsFile;
            dir = Path.GetDirectoryName(fileName);
        }


        if (typeof(OutputProfileDict).IsAssignableFrom(setting.GetType()))
        {
            fileName = BuildInfo.OutputProfileSettingsFile;
            dir = Path.GetDirectoryName(fileName);
        }

        //if (typeof(HLSSettings).IsAssignableFrom(setting.GetType()))
        //{
        //    fileName = BuildInfo.HLSSettingsFile;
        //    dir = Path.GetDirectoryName(fileName);
        //}

        if (typeof(SDSettings).IsAssignableFrom(setting.GetType()))
        {
            fileName = BuildInfo.SDSettingsFile;
            dir = Path.GetDirectoryName(fileName);
        }

        if (string.IsNullOrEmpty(dir))
        {
            return;
        }

        if (!Directory.Exists(dir))
        {
            _ = Directory.CreateDirectory(dir);
        }

        string jsonString = JsonSerializer.Serialize(setting, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fileName, jsonString);
    }
}
