namespace StreamMaster.Application.SMChannels;

public class SMChannelService(IRepositoryWrapper repositoryWrapper) : ISMChannelService
{
    public async Task<List<NameLogo>> GetNameLogos()
    {
        List<NameLogo> channelNames = await repositoryWrapper.SMChannel.GetQuery().OrderBy(a => a.Name).Select(a => new NameLogo(a)).ToListAsync().ConfigureAwait(false);
        return channelNames;
    }
}
