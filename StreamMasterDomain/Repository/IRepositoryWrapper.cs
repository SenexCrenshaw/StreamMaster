namespace StreamMasterDomain.Repository
{
    public interface IRepositoryWrapper
    {
        IM3UFileRepository M3UFile { get; }
        Task SaveAsync();
    }
}
