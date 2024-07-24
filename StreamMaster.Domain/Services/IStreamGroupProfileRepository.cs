using StreamMaster.Domain.Repository;

namespace StreamMaster.Domain.Services
{
    public interface IStreamGroupProfileRepository : IRepositoryBase<StreamGroupProfile>
    {
        Task<StreamGroupProfile> GetStreamGroupProfileAsync(int? StreamGroupId = null, int? StreamGroupProfileId = null);

        Task DeleteStreamGroupProfile(StreamGroupProfile StreamGroupProfile);
        Task<List<StreamGroupProfile>> GetStreamGroupProfiles(int? StreamGroupId = null);
        void Update(StreamGroupProfile streamGroupProfile);
    }
}