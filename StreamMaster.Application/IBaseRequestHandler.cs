using StreamMaster.Domain.Configuration;

namespace StreamMaster.Application
{
    public interface IBaseRequestHandler
    {
        Task<Setting> GetSettings();
    }
}