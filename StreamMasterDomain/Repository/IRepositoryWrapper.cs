namespace StreamMasterDomain.Repository
{
    public interface IRepositoryWrapper
    {
        IEPGFileRepository EPGFile { get; }
        IChannelGroupRepository ChannelGroup { get; }
        IStreamGroupRepository StreamGroup { get; }
        IM3UFileRepository M3UFile { get; }
        IVideoStreamRepository VideoStream { get; }
        IVideoStreamLinkRepository VideoStreamLink { get; }
        IStreamGroupChannelGroupRepository StreamGroupChannelGroup { get; }
        IStreamGroupVideoStreamRepository StreamGroupVideoStream { get; }
        Task<int> SaveAsync();
    }
}