namespace StreamMasterDomain.Services;

public interface ISettingsService
{
    Task<Setting> GetSettingsAsync();
    Task UpdateSettingsAsync(Setting settings);
}
