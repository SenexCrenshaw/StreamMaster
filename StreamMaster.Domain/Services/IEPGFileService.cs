using StreamMaster.Domain.API;

namespace StreamMaster.Domain.Services
{
    public interface IEPGFileService
    {
        Task<DataResponse<List<EPGFileDto>>> GetEPGFilesNeedUpdatingAsync();
    }
}