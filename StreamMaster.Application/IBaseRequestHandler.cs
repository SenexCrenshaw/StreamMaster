using StreamMaster.Domain.Common;

namespace StreamMaster.Application
{
    public interface IBaseRequestHandler
    {
        Task<Setting> GetSettings();
    }
}