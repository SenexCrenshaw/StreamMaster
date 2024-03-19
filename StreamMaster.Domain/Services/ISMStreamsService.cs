using StreamMaster.Domain.API;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.Services
{
    public interface ISMStreamsService
    {
        Task<APIResponse<SMStreamDto>> GetPagedSMStreams(SMStreamParameters Parameters);
        Task<DefaultAPIResponse> ToggleSMStreamVisibleById(string id);
    }
}