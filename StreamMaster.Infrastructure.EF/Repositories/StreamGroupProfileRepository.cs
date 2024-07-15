namespace StreamMaster.Infrastructure.EF.Repositories;

public class StreamGroupProfileRepository(ILogger<StreamGroupProfileRepository> intLogger, IRepositoryContext repositoryContext)
    : RepositoryBase<StreamGroupProfile>(repositoryContext, intLogger), IStreamGroupProfileRepository
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
        StreamGroupProfile? profile = GetQuery().FirstOrDefault(a => a.StreamGroupId == StreamGroupId && a.Id == StreamGroupProfileId);

        return profile ?? GetQuery().FirstOrDefault(a => a.StreamGroupId == StreamGroupId && a.OutputProfileName == "Default");
    }
}
