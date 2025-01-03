namespace StreamMaster.Domain.Services
{
    public interface IDataRefreshService: IDataRefreshServicePartial
    {
        Task SetField(List<FieldData> fieldData);
        Task ClearByTag(string Entity, string Tag);
        Task RefreshAll();
        Task Refresh(string command);
        Task RefreshDownloadServiceStatus();
        Task RefreshVs(bool alwaysRun = false);
        Task RefreshStreamGroups(bool alwaysRun = false);
        Task RefreshStreamGroupSMChannelLinks(bool alwaysRun = false);
        Task RefreshStatistics(bool alwaysRun = false);
        Task RefreshSMTasks(bool alwaysRun = false);
        Task RefreshSMStreams(bool alwaysRun = false);
        Task RefreshSMChannels(bool alwaysRun = false);
        Task RefreshSMChannelStreamLinks(bool alwaysRun = false);
        Task RefreshSMChannelChannelLinks(bool alwaysRun = false);
        Task RefreshSettings(bool alwaysRun = false);
        Task RefreshSchedulesDirect(bool alwaysRun = false);
        Task RefreshProfiles(bool alwaysRun = false);
        Task RefreshM3UFiles(bool alwaysRun = false);
        Task RefreshLogs(bool alwaysRun = false);
        Task RefreshLogos(bool alwaysRun = false);
        Task RefreshEPG(bool alwaysRun = false);
        Task RefreshEPGFiles(bool alwaysRun = false);
        Task RefreshCustom(bool alwaysRun = false);
        Task RefreshChannelGroups(bool alwaysRun = false);
    }
}
