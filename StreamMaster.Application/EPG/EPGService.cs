namespace StreamMaster.Application.EPG;

public class EPGService(IRepositoryWrapper repositoryWrapper) : IEPGService
{
    public async Task<List<EPGFile>> GetEPGFilesAsync()
    {
        return await repositoryWrapper.EPGFile.GetQuery().ToListAsync();
    }
}