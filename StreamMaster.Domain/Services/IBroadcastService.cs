namespace StreamMaster.Domain.Services;

public interface IBroadcastService
{
    void StartBroadcasting();
    void StopBroadcasting();
}
