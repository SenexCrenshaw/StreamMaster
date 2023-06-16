using StreamMasterDomain.Authentication;
using StreamMasterDomain.Configuration;
using StreamMasterDomain.Entities;
using StreamMasterDomain.EnvironmentInfo;
using StreamMasterDomain.Extensions;

namespace StreamMasterInfrastructure.Authentication;

public interface IUserService
{
    User Add(string username, string password);

    User FindUser(string username, string password);

    User FindUser(Guid identifier);

    User Upsert(string username, string password);
}

public class UserService : IUserService
{
    private readonly IAppFolderInfo _appFolderInfo;
    private readonly IConfigFileProvider _configFileProvider;
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo,
        IAppFolderInfo appFolderInfo,
        IConfigFileProvider configFileProvider)
    {
        _repo = repo;
        _appFolderInfo = appFolderInfo;
        _configFileProvider = configFileProvider;
    }

    public User Add(string username, string password)
    {
        return _repo.Insert(new User
        {
            Identifier = Guid.NewGuid(),
            Username = username.ToLowerInvariant(),
            Password = password.SHA256Hash()
        });
    }

    public User FindUser(string username, string password)
    {
        if (username.IsNullOrWhiteSpace() || password.IsNullOrWhiteSpace())
        {
            return null;
        }

        var user = _repo.FindUser(username.ToLowerInvariant());

        if (user == null)
        {
            return null;
        }

        if (user.Password == password.SHA256Hash())
        {
            return user;
        }

        return null;
    }

    public User FindUser(Guid identifier)
    {
        return _repo.FindUser(identifier);
    }

    public User Upsert(string username, string password)
    {
        var user = _repo.FindUser(username.ToLowerInvariant());

        if (user == null)
        {
            return Add(username, password);
        }

        if (user.Password != password)
        {
            user.Password = password.SHA256Hash();
        }

        _repo.SaveChanges();

        return user;
    }
}
