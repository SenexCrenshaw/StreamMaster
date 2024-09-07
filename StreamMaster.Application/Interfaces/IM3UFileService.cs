namespace StreamMaster.Application.Interfaces;
public interface IM3UFileService
{
    Task<M3UFile?> GetM3UFileAsync(int Id);
    Task<M3UFile?> ProcessM3UFile(int M3UFileId, bool ForceRun = false);
    Task UpdateM3UFile(M3UFile m3uFile);
    Task<List<M3UFile>> GetM3UFilesAsync();
}