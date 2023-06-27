namespace StreamMasterApplication.Common.Interfaces;

public interface IAppDbContext : ISharedDB
{
      int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
