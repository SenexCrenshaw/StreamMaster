using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.M3UFiles;

using StreamMasterDomain.Entities;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext : IM3UFileDB
{
    public DbSet<M3UFile> M3UFiles { get; set; }
}