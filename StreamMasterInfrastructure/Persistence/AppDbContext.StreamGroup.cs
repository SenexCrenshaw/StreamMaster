using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.StreamGroups;

using StreamMasterDomain.Entities;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext : IStreamGroupDB
{
    public DbSet<StreamGroup> StreamGroups { get; set; }
    //public DbSet<StreamGroupVideoStream> StreamGroupVideoStreams { get; set; }
}
