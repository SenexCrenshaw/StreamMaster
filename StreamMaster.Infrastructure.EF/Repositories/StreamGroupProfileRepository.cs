using Microsoft.EntityFrameworkCore;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class StreamGroupProfileRepository(ILogger<StreamGroupProfileRepository> intLogger, IRepositoryContext repositoryContext)
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

    public async Task<List<StreamGroupProfile>> GetStreamGroupProfiles(int? StreamGroupId = null)
    {
        return StreamGroupId.HasValue
            ? await GetQuery().Where(a => a.StreamGroupId == StreamGroupId).ToListAsync()
            : await GetQuery().ToListAsync();
    }
}
