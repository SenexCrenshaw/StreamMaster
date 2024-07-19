using AutoMapper;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class StreamGroupProfileRepository(ILogger<StreamGroupProfileRepository> intLogger, IMapper mapper, IRepositoryContext repositoryContext)
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

    public async Task<StreamGroupProfileDto> GetDefaultStreamGroupProfile(int StreamGroupId)
    {
        StreamGroupProfile? profile = GetQuery().FirstOrDefault(a => a.Name == "Default");

        profile ??= GetQuery().Where(a => a.StreamGroupId == StreamGroupId).OrderBy(a => a.Id).FirstOrDefault();

        if (profile == null)
        {
            profile = new StreamGroupProfile
            {
                StreamGroupId = StreamGroupId,
                Name = "Default",
                OutputProfileName = "Default",
                CommandProfileName = "Default"
            };
            Create(profile);
            await SaveChangesAsync();
        }

        var dto = mapper.Map<StreamGroupProfileDto>(profile);

        return dto;

    }

    public StreamGroupProfile? GetStreamGroupProfile(int StreamGroupId, int StreamGroupProfileId)
    {
        StreamGroupProfile? profile = GetQuery().FirstOrDefault(a => a.StreamGroupId == StreamGroupId && a.Id == StreamGroupProfileId);

        return profile ?? GetQuery().FirstOrDefault(a => a.StreamGroupId == StreamGroupId && a.OutputProfileName == "Default");
    }
}
