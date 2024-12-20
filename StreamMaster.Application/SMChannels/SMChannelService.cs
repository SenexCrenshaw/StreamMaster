namespace StreamMaster.Application.SMChannels;

public class SMChannelService(IRepositoryWrapper repositoryWrapper) : ISMChannelService
{
    //public IQueryable<LogoInfo > GetlogoInfos()
    //{
    //    IQueryable<LogoInfo > channelNames = repositoryWrapper.SMChannel.GetQuery()
    //        .SelectMany(a => a.SMStreams.Select(smStream => new LogoInfo (smStream.SMStream)));
    //    return channelNames;
    //}

    public IQueryable<SMChannel> GetSMStreamLogos(bool? justHttp = true)
    {
        return justHttp == true
            ? repositoryWrapper.SMChannel.GetQuery().Where(a => a.Logo.Contains("://"))
            : repositoryWrapper.SMChannel.GetQuery().Where(a => !string.IsNullOrEmpty(a.Logo));
    }
}
