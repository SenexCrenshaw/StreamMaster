using StreamMaster.Domain.Common;

namespace StreamMaster.Domain.Services;

public interface ISettingsService
{
    Task<Setting> GetSettingsAsync(CancellationToken cancellationToken = default);
    Task UpdateSettingsAsync(Setting settings);
}
