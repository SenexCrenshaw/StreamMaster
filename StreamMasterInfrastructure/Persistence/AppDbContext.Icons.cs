using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Icons;

using StreamMasterDomain.Entities;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext : IIconDB
{
    public DbSet<IconFile> Icons { get; set; }
}