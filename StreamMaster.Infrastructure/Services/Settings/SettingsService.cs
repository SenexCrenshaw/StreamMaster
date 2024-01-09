using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Services;

using System.Text.Json;

namespace StreamMaster.Infrastructure.Services.Settings;

public class SettingsService : ISettingsService
{
    private IChangeToken _fileChangeToken;
    private readonly PhysicalFileProvider _fileProvider;

    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly IMemoryCache memoryCache;
    private readonly ILogger<SettingsService> logger;

    public SettingsService(IMemoryCache memoryCache, ILogger<SettingsService> logger)
    {
        this.memoryCache = memoryCache;
        this.logger = logger;
        _fileProvider = new PhysicalFileProvider(BuildInfo.AppDataFolder)
        {
            UsePollingFileWatcher = true,
            UseActivePolling = true
        };
        GetSettingsAsync().Wait();
        WatchForFileChanges();
    }

    private void WatchForFileChanges()
    {
        _fileChangeToken = _fileProvider.Watch(BuildInfo.SettingFileName);
        _fileChangeToken.RegisterChangeCallback(Notify, default);
    }

    private void Notify(object? state)
    {
        logger.LogInformation("Settings changed, reloading");
        GetSettingsAsync().Wait();
        WatchForFileChanges();
    }

    public async Task<Setting> GetSettingsAsync(CancellationToken cancellationToken = default)
    {

        await _semaphore.WaitAsync(cancellationToken);
        Setting? settings = null;
        try
        {
            if (!File.Exists(BuildInfo.SettingFile))
            {
                await UpdateSettingsAsync(new());
            }
            using (FileStream fs = File.OpenRead(BuildInfo.SettingFile))
            {
                settings = await JsonSerializer.DeserializeAsync<Setting>(fs, cancellationToken: cancellationToken);
            }

            if (settings == null)
            {
                throw new InvalidOperationException("Failed to load settings from file.");
            }

            memoryCache.SetSetting(settings);

        }
        finally
        {
            _semaphore.Release();
        }


        return settings ?? throw new InvalidOperationException("Failed to retrieve settings.");
    }


    public async Task UpdateSettingsAsync(Setting newSettings)
    {
        await _semaphore.WaitAsync();

        try
        {
            // Directly use FileStream to write JSON
            using FileStream fs = File.Create(BuildInfo.SettingFile);
            await JsonSerializer.SerializeAsync(fs, newSettings);

            // Update the cache
            MemoryCacheEntryOptions cacheEntryOptions = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            };
            memoryCache.SetSetting(newSettings);
        }
        finally
        {
            _semaphore.Release();
        }
    }

}