namespace StreamMaster.Application.SMChannels;

public class SMChannelService(IRepositoryWrapper repositoryWrapper) : ISMChannelService
{
    public IQueryable<NameLogo> GetNameLogos()
    {
        IQueryable<NameLogo> channelNames = repositoryWrapper.SMChannel.GetQuery().OrderBy(a => a.Name).Select(a => new NameLogo(a, SMFileTypes.Logo));
        return channelNames;
    }
}
