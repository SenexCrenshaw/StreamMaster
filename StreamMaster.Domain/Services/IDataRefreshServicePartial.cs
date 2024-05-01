namespace StreamMaster.Application.Common.Interfaces
{
    public interface IDataRefreshServicePartial
    {
        Task RefreshAllEPG();
        Task RefreshAllM3U();
        Task RefreshAllSMChannels();
    }
}
