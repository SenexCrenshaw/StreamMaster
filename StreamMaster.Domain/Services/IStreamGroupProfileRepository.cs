using StreamMaster.Domain.Repository;

namespace StreamMaster.Domain.Services
{
    public interface IStreamGroupProfileRepository : IRepositoryBase<StreamGroupProfile>
    {
        StreamGroupProfile? GetStreamGroupProfile(int StreamGroupId, int StreamGroupProfileId);
        Task DeleteStreamGroupProfile(StreamGroupProfile StreamGroupProfile);
        //List<StreamGroupProfile> GetStreamGroupProfiles();
        void Update(StreamGroupProfile streamGroupProfile);
    }
}