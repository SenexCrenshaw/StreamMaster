namespace StreamMasterDomain.Services;

public interface ISettingsService
{
    Task<Setting> GetSettingsAsync(CancellationToken cancellationToken = default);
    Task UpdateSettingsAsync(Setting settings);
}
