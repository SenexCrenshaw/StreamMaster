namespace StreamMaster.Domain.Services
{
    public interface ISMStreamService
    {
        IQueryable<SMStream> GetSMStreamLogos(bool? justHttp = true);
    }
}