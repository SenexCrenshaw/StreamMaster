namespace StreamMaster.Domain.Services
{
    public interface ISMChannelService
    {
        Task<List<NameLogo>> GetNameLogos();
    }
}