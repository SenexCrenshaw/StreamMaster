using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Users;

using StreamMasterDomain.Entities;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext : IUsersDB
{
    public DbSet<User> Users { get; set; }
}
