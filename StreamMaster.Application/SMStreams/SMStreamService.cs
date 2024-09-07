namespace StreamMaster.Application.SMStreams;

public class SMStreamService(ILogger<ISMStreamService> logger, IRepositoryWrapper repositoryWrapper)
    : ISMStreamService
{
    public IQueryable<SMStream> GetSMStreamLogos(bool? justHttp = true)
    {
        return justHttp == true
            ? repositoryWrapper.SMStream.GetQuery().Where(a => a.Logo.Contains("://"))
            : repositoryWrapper.SMStream.GetQuery().Where(a => !string.IsNullOrEmpty(a.Logo));
    }
}
