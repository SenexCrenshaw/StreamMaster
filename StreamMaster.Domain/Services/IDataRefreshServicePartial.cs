namespace StreamMaster.Domain.Services
{
    public interface IDataRefreshServicePartial
    {
        Task IsSystemReady();
        Task TaskIsRunning();
        Task SendMessage(SMMessage smMessage);
        Task SendSMTasks(List<SMTask> smTasks);
        Task RefreshAllEPG();
        Task RefreshAllM3U();
        Task RefreshAllSMChannels();
    }
}
