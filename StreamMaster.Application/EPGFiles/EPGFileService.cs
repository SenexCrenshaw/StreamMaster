namespace StreamMaster.Application.EPGFiles;

public class EPGFileService(IRepositoryWrapper repositoryWrapper) : IEPGFileService
{
    public async Task<DataResponse<List<EPGFileDto>>> GetEPGFilesNeedUpdatingAsync()
    {
        List<EPGFileDto> epgFiles = await repositoryWrapper.EPGFile.GetEPGFilesNeedUpdatingAsync();
        return DataResponse<List<EPGFileDto>>.Success(epgFiles);
    }
}
