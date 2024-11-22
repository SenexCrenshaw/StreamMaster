namespace StreamMaster.Application.SMChannels;

public class SMChannelService(IRepositoryWrapper repositoryWrapper) : ISMChannelService
{
    public IQueryable<NameLogo> GetNameLogos()
    {
        IQueryable<NameLogo> channelNames = repositoryWrapper.SMChannel.GetQuery().SelectMany(a => a.SMStreams.Select(smStream => new NameLogo(smStream.SMStream)));
        return channelNames;
    }
}
