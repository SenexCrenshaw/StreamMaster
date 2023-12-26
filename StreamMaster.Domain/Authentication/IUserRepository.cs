using StreamMaster.Domain.Models;

namespace StreamMaster.Domain.Authentication;

public interface IUserRepository
{
    User? FindUser(string username);

    User? FindUser(Guid identifier);

    User Insert(User user);

    int SaveChanges();
}
