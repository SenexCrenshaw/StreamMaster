using StreamMasterDomain.Authentication;
using StreamMasterDomain.EnvironmentInfo;
using StreamMasterDomain.Extensions;

using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace StreamMasterDomain.Configuration
{
    public interface IConfigFileProvider
    {
        Setting Setting { get; }

        void UpdateSetting(Setting setting);
    }

    public class ConfigFileProvider : IConfigFileProvider
    {
        private readonly string _configFile;
        private readonly IAppFolderInfo _appFolderInfo;
        private Setting currentSetting;
        private FileSystemWatcher _fileWatcher;

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

            if (currentSetting.ServerKey.IsNullOrWhiteSpace())
            {
                currentSetting.ServerKey = GenerateApiKey();
                UpdateSetting();
            }

            if (Debugger.IsAttached)
            {
                currentSetting.UiFolder = "devwwwroot";
                currentSetting.ApiKey = "f835904d5a2343d8ac567c026d6c08b2";
                currentSetting.ServerKey = "ef1a2b3c4d5e6f7a8b9c0d1e2f3a4b5c";
            }

            // Start file watcher
            StartFileWatcher();
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
                //jsonString = File.ReadAllText(_configFile);

                using(var fileStream = new FileStream(_configFile, FileMode.Open, FileAccess.Read, FileShare.Read))
{
                    using (var reader = new StreamReader(fileStream))
                    {
                        jsonString = reader.ReadToEnd();
                    }
                }

                var ret = JsonSerializer.Deserialize<Setting>(jsonString);
                if (ret != null)
                {
                    currentSetting = ret;

                    if (Debugger.IsAttached)
                    {
                        currentSetting.UiFolder = "devwwwroot";
                        currentSetting.ApiKey = "f835904d5a2343d8ac567c026d6c08b2";
                        currentSetting.ServerKey = "ef1a2b3c4d5e6f7a8b9c0d1e2f3a4b5c";
                    }

                    return;
                }
            }

            currentSetting = new Setting();
            UpdateSetting(currentSetting);
        }

        private void StartFileWatcher()
        {
            _fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(_configFile), Path.GetFileName(_configFile));
            _fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _fileWatcher.Changed += ConfigFileChanged;
            _fileWatcher.EnableRaisingEvents = true;
        }

        private void ConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            // Reload settings when the config file is modified
            LoadSettings();
        }
    }
}
