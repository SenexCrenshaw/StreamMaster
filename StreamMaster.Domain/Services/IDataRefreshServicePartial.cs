namespace StreamMaster.Domain.Services
{
    public interface IDataRefreshServicePartial
    {
        Task RefreshAllEPG();
        Task RefreshAllM3U();
        Task RefreshAllSMChannels();
    }
}
