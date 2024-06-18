namespace StreamMaster.Domain.Services
{
    public interface IStreamGroupProfileRepository
    {
        void DeleteStreamGroupProfile(StreamGroupProfile test);
        List<StreamGroupProfile> GetStreamGroupProfiles();
        void Update(StreamGroupProfile streamGroupProfile);
    }
}