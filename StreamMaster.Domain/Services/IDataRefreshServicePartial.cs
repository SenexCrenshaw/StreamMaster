namespace StreamMaster.Domain.Services
{
    public interface IDataRefreshServicePartial
    {
        Task RefreshSelectedStationIds();
        Task RefreshStationPreviews();
        Task RefreshOutputProfiles();
        Task RefreshCommandProfiles();
        Task IsSystemReady();
        Task TaskIsRunning();
        Task SendMessage(SMMessage smMessage);
        Task SendSMTasks(List<SMTask> smTasks);
        Task RefreshAllEPG();
        Task RefreshAllM3U();
        Task RefreshAllSMChannels();
    }
}
