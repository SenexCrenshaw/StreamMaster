namespace StreamMasterApplication.Common.Interfaces;

public interface IAppDbContext :
    ITaskDB,
    ISharedDB
{
    ValueTask ResetDBAsync(CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
