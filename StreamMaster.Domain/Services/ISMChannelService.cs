namespace StreamMaster.Domain.Services
{
    public interface ISMChannelService
    {
        IQueryable<NameLogo> GetNameLogos();
    }
}