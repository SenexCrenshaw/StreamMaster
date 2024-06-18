using StreamMaster.Domain.Configuration;
namespace StreamMaster.Infrastructure.EF.Repositories;

public class StreamGroupProfileRepository(ILogger<StreamGroupProfileRepository> intLogger, IRepositoryContext repositoryContext, IOptionsMonitor<Setting> intSettings)
    : RepositoryBase<StreamGroupProfile>(repositoryContext, intLogger, intSettings), IStreamGroupProfileRepository
{
    public void DeleteStreamGroupProfile(StreamGroupProfile test)
    {
        Delete(test);
    }

    public List<StreamGroupProfile> GetStreamGroupProfiles()
    {
        return [.. GetQuery()];
    }

}
