namespace StreamMaster.Domain.Services
{
    public interface IStreamGroupProfileRepository
    {
        StreamGroupProfile? GetStreamGroupProfile(int StreamGroupId, int StreamGroupProfileId);
        void DeleteStreamGroupProfile(StreamGroupProfile StreamGroupProfile);
        List<StreamGroupProfile> GetStreamGroupProfiles();
        void Update(StreamGroupProfile streamGroupProfile);
    }
}