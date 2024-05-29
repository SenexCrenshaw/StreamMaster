using StreamMaster.Domain.Services;

namespace StreamMaster.Domain.Repository
{
    public interface IRepositoryWrapper
    {
        IStreamGroupSMChannelLinkRepository StreamGroupSMChannelLink { get; }
        ISMChannelsRepository SMChannel { get; }
        ISMStreamRepository SMStream { get; }
        IEPGFileRepository EPGFile { get; }
        IChannelGroupRepository ChannelGroup { get; }
        IStreamGroupRepository StreamGroup { get; }
        IM3UFileRepository M3UFile { get; }
        ISMChannelStreamLinksRepository SMChannelStreamLink { get; }
        Task<int> SaveAsync();
    }
}