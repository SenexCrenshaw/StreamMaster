namespace StreamMasterApplication.Common.Interfaces;

public interface IAppDbContext : ISharedDB
{
    ValueTask ResetDBAsync(CancellationToken cancellationToken = default);

    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
