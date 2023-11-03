namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

public interface ISDToken
{
    Task<string> GetAPIUrl(string command, CancellationToken cancellationToken);
    Task<string?> GetToken(CancellationToken cancellationToken = default);
    Task LockOutToken(int minutes = 15, CancellationToken cancellationToken = default);
    Task<string?> ResetToken(CancellationToken cancellationToken = default);
}