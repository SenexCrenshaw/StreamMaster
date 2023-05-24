using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.EPGFiles;

using StreamMasterDomain.Entities;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext : IEPGFileDB
{
    public DbSet<EPGFile> EPGFiles { get; set; }
}
