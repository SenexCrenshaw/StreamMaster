namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

public interface ISDTokenFileHandler
{
    string? LoadToken();
    void SaveToken(string token);
}