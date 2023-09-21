namespace StreamMasterApplication
{
    public interface IBaseRequestHandler
    {
        Task<Setting> GetSettings();
    }
}