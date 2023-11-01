using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

public interface ISDTokenCache
{
    public ISDStatus? GetStatus();
    public void SetStatus(ISDStatus status);
}