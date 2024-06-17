namespace StreamMaster.Domain.Services
{
    public interface IDataRefreshService: IDataRefreshServicePartial
    {
        Task SetField(List<FieldData> fieldData);
        Task ClearByTag(string Entity, string Tag);
        Task RefreshAll();
        Task Refresh(string command);
        Task RefreshStreamGroups(bool alwaysRun = false);
        Task RefreshStreamGroupSMChannelLinks(bool alwaysRun = false);
        Task RefreshSMTasks(bool alwaysRun = false);
        Task RefreshSMStreams(bool alwaysRun = false);
        Task RefreshSMChannels(bool alwaysRun = false);
        Task RefreshSMChannelStreamLinks(bool alwaysRun = false);
        Task RefreshSettings(bool alwaysRun = false);
        Task RefreshSchedulesDirect(bool alwaysRun = false);
        Task RefreshM3UFiles(bool alwaysRun = false);
        Task RefreshIcons(bool alwaysRun = false);
        Task RefreshGeneral(bool alwaysRun = false);
        Task RefreshEPG(bool alwaysRun = false);
        Task RefreshEPGFiles(bool alwaysRun = false);
        Task RefreshChannelGroups(bool alwaysRun = false);
    }
}
