using StreamMaster.Domain.Requests;

namespace StreamMaster.Domain.Services;

public interface IM3UFileService
{
    Task<DataResponse<List<M3UFileDto>>> GetM3UFilesNeedUpdatingAsync();

    Task<M3UFile?> GetM3UFileAsync(int Id);

    Task<M3UFile?> ProcessM3UFile(int M3UFileId, bool ForceRun = false);

    Task UpdateM3UFile(M3UFile m3uFile);

    Task<List<M3UFile>> GetM3UFilesAsync();

    (M3UFile m3uFile, string fullName) CreateM3UFile(CreateM3UFileRequest request);

    (M3UFile m3uFile, string fullName) CreateM3UFile(CreateM3UFileFromFormRequest request);

    (string fullName, string fullNameWithExtension) GetFileName(string name);
}