using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.ChannelGroups;

using StreamMasterDomain.Entities;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext : IChannelGroupDB
{
    public DbSet<ChannelGroup> ChannelGroups { get; set; }
}
