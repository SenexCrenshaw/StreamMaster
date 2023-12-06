namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

public interface ISDToken
{
    Task<string?> GetAPIUrl(string command, CancellationToken cancellationToken);
    Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);
    Task LockOutTokenAsync(int minutes = 15, CancellationToken cancellationToken = default);
    Task<string?> ResetTokenAsync(CancellationToken cancellationToken = default);
}