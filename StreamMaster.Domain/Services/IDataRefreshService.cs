namespace StreamMaster.Domain.Services
{
    public interface IDataRefreshService: IDataRefreshServicePartial
    {
        Task RefreshAll();
        Task RefreshStreamGroups();
        Task RefreshStreamGroupSMChannelLinks();
        Task RefreshSMStreams();
        Task RefreshSMChannels();
        Task RefreshSMChannelStreamLinks();
        Task RefreshSettings();
        Task RefreshSchedulesDirect();
        Task RefreshM3UFiles();
        Task RefreshIcons();
        Task RefreshEPG();
        Task RefreshEPGFiles();
        Task RefreshChannelGroups();
    }
}
