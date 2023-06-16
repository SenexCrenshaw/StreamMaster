using Microsoft.EntityFrameworkCore;

namespace StreamMasterApplication.Users;

public interface IUsersController
{
}

public interface IUsersDB
{
    DbSet<User> Users { get; set; }
}

public interface IUsersHub
{
}

public interface IUsersTasks
{
}
