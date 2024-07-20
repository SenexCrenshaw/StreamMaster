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

    public async Task<StreamGroupProfileDto> GetDefaultStreamGroupProfile(int StreamGroupId)
    {
        StreamGroupProfile? profile = GetQuery().FirstOrDefault(a => a.ProfileName == "Default");

        profile ??= GetQuery().Where(a => a.StreamGroupId == StreamGroupId).OrderBy(a => a.Id).FirstOrDefault();

        if (profile == null)
        {
            profile = new StreamGroupProfile
            {
                StreamGroupId = StreamGroupId,
                ProfileName = "Default",
                OutputProfileName = "Default",
                CommandProfileName = "Default"
            };
            Create(profile);
            _ = await SaveChangesAsync();
        }

        StreamGroupProfileDto dto = mapper.Map<StreamGroupProfileDto>(profile);

        return dto;

    }

    public StreamGroupProfile? GetStreamGroupProfile(int StreamGroupId, int StreamGroupProfileId)
    {
        StreamGroupProfile? profile = GetQuery().FirstOrDefault(a => a.StreamGroupId == StreamGroupId && a.Id == StreamGroupProfileId);

        return profile ?? GetQuery().FirstOrDefault(a => a.StreamGroupId == StreamGroupId && a.OutputProfileName == "Default");
    }
}
