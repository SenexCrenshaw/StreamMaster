
namespace StreamMaster.Domain.Services
{
    public interface ISMChannelService
    {
        //IQueryable<LogoInfo > GetlogoInfos();
        IQueryable<SMChannel> GetSMStreamLogos(bool? justHttp = true);
    }
}