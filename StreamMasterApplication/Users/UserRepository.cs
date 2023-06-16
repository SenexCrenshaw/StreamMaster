using StreamMasterDomain.Authentication;

namespace StreamMasterApplication.Users;

public class UserRepository : IUserRepository
{
    private readonly IAppDbContext _context;

    public UserRepository(IAppDbContext context)
    {
        _context = context;
    }

    public User? FindUser(string username)
    {
        return _context.Users.SingleOrDefault(x => x.Username == username);
    }

    public User? FindUser(Guid identifier)
    {
        return _context.Users.SingleOrDefault(x => x.Identifier == identifier);
    }

    public User Insert(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    private User? FindUser(User user)
    {
        return _context.Users.SingleOrDefault(x => x.Username == user.Username);
    }
}
