namespace StreamMasterDomain.Repository
{
    public interface IRepositoryWrapper
    {
        IEPGFileRepository EPGFile { get; }
        IChannelGroupRepository ChannelGroup { get; }
        IStreamGroupRepository StreamGroup { get; }
        IM3UFileRepository M3UFile { get; }
        IVideoStreamRepository VideoStream { get; }
        Task<int> SaveAsync();
    }
}
