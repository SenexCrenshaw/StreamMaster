using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces
{
    public interface ISDTokenService
    {
        Task<ISDStatus?> GetStatusAsync(CancellationToken cancellationToken);
        Task<string?> GetTokenAsync(CancellationToken cancellationToken);
    }
}