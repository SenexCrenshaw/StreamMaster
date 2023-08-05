namespace StreamMasterDomain.Repository
{
    public interface IRepositoryWrapper
    {
        IM3UFileRepository M3UFile { get; }
        IVideoStreamRepository VideoStream { get; }
        Task SaveAsync();
    }
}
