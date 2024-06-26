using StreamMaster.Domain.Configuration;
namespace StreamMaster.Infrastructure.EF.Repositories;

public class StreamGroupProfileRepository(ILogger<StreamGroupProfileRepository> intLogger, IRepositoryContext repositoryContext, IOptionsMonitor<Setting> intSettings)
    : RepositoryBase<StreamGroupProfile>(repositoryContext, intLogger, intSettings), IStreamGroupProfileRepository
{
    public void DeleteStreamGroupProfile(StreamGroupProfile StreamGroupProfile)
    {
        Delete(StreamGroupProfile);
    }

    public List<StreamGroupProfile> GetStreamGroupProfiles()
    {
        return [.. GetQuery()];
    }

    public StreamGroupProfile? GetStreamGroupProfile(int StreamGroupId, int StreamGroupProfileId)
    {
        var profile = GetQuery().FirstOrDefault(a => a.StreamGroupId == StreamGroupId && a.Id == StreamGroupProfileId);

        if (profile == null)
        {
            return GetQuery().FirstOrDefault(a => a.StreamGroupId == StreamGroupId && a.OutputProfileName == "Default");
        }

        return profile;
    }

}
