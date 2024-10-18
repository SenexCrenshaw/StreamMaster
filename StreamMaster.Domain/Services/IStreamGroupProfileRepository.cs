using StreamMaster.Domain.Repository;

namespace StreamMaster.Domain.Services
{
    public interface IStreamGroupProfileRepository : IRepositoryBase<StreamGroupProfile>
    {
        Task DeleteStreamGroupProfile(StreamGroupProfile StreamGroupProfile);
        Task<List<StreamGroupProfile>> GetStreamGroupProfiles(int? StreamGroupId = null);
        void Update(StreamGroupProfile streamGroupProfile);
    }
}