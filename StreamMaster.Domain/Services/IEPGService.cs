namespace StreamMaster.Domain.Services
{
    public interface IEPGService
    {
        Task<List<EPGFile>> GetEPGFilesAsync();
    }
}