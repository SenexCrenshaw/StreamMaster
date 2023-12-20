using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Common;
using StreamMasterDomain.Services;

using System.Text.Json;

namespace StreamMasterInfrastructure.Services.Settings;

public class SettingsService(IMemoryCache cache) : ISettingsService
{
    private readonly string _settingsFilePath = BuildInfo.SettingFile;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<Setting> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
        if (!cache.TryGetValue("Setting", out Setting? settings))
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                if (!cache.TryGetValue("Setting", out settings))
                {
                    if (!File.Exists(_settingsFilePath))
                    {
                        await UpdateSettingsAsync(new());
                    }
                    using (FileStream fs = File.OpenRead(_settingsFilePath))
                    {
                        settings = await JsonSerializer.DeserializeAsync<Setting>(fs, cancellationToken: cancellationToken);
                    }

                    if (settings == null)
                    {
                        throw new InvalidOperationException("Failed to load settings from file.");
                    }

                    MemoryCacheEntryOptions cacheEntryOptions = new()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                    };
                    cache.Set("Setting", settings, cacheEntryOptions);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        return settings == null ? throw new InvalidOperationException("Failed to retrieve settings from cache.") : settings;
    }


    public async Task UpdateSettingsAsync(Setting newSettings)
    {
        await _semaphore.WaitAsync();

        try
        {
            // Directly use FileStream to write JSON
            using FileStream fs = File.Create(_settingsFilePath);
            await JsonSerializer.SerializeAsync(fs, newSettings);

            // Update the cache
            MemoryCacheEntryOptions cacheEntryOptions = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            };
            cache.Set("Setting", newSettings, cacheEntryOptions);
        }
        finally
        {
            _semaphore.Release();
        }
    }

}