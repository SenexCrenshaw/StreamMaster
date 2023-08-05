using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.EPGFiles;

using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructure.EF;

public partial class AppDbContext : IEPGFileDB
{
    public DbSet<EPGFile> EPGFiles { get; set; }
}
