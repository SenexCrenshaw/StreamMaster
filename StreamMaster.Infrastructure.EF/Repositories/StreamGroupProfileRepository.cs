using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Application.StreamGroups.Queries;
using StreamMaster.Domain.API;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class StreamGroupProfileRepository(ILogger<StreamGroupProfileRepository> intLogger, ISender sender, IRepositoryContext repositoryContext)
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

    public async Task<StreamGroupProfile> GetStreamGroupProfileAsync(int? StreamGroupId = null, int? StreamGroupProfileId = null)
    {
        if (StreamGroupProfileId.HasValue)
        {
            StreamGroupProfile? profile = GetQuery().FirstOrDefault(a => a.Id == StreamGroupProfileId);
            if (profile != null)
            {
                return profile;
            }
        }

        if (StreamGroupId.HasValue)
        {
            StreamGroupProfile? profile = GetQuery().FirstOrDefault(a => a.StreamGroupId == StreamGroupId && a.ProfileName == "Default");
            if (profile != null)
            {
                return profile;
            }
        }

        DataResponse<StreamGroupProfile> def = await sender.Send(new GetDefaultStreamGroupProfileIdRequest());
        return def.Data;
    }

    public async Task<List<StreamGroupProfile>> GetStreamGroupProfiles(int? StreamGroupId = null)
    {
        return StreamGroupId.HasValue
            ? await GetQuery().Where(a => a.StreamGroupId == StreamGroupId).ToListAsync()
            : await GetQuery().ToListAsync();
    }
}
