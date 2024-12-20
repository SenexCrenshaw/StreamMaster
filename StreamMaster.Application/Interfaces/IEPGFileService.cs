using StreamMaster.Application.EPGFiles.Commands;

namespace StreamMaster.Application.Interfaces
{
    public interface IEPGFileService
    {
        Task<List<EPGFile>> GetEPGFilesAsync();
        Task<(EPGFile epgFile, string fullName)> CreateEPGFileAsync(CreateEPGFileRequest request);

        Task<(EPGFile epgFile, string fullName)> CreateEPGFileAsync(CreateEPGFileFromFormRequest request);

        Task<DataResponse<List<EPGFileDto>>> GetEPGFilesNeedUpdatingAsync();
        (string fullName, string fullNameWithExtension) GetFileName(string name);
    }
}