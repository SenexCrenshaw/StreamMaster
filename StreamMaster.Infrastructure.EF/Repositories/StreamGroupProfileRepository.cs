using AutoMapper;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class StreamGroupProfileRepository(ILogger<StreamGroupProfileRepository> intLogger, IMapper mapper, IRepositoryContext repositoryContext)
    : RepositoryBase<StreamGroupProfile>(repositoryContext, intLogger), IStreamGroupProfileRepository
{
    public async Task DeleteStreamGroupProfile(StreamGroupProfile StreamGroupProfile)
    {
        Delete(StreamGroupProfile);
        _ = await SaveChangesAsync();
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
