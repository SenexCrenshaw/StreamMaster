namespace StreamMaster.Domain.Services
{
    public interface IStreamGroupProfileRepository
    {
        StreamGroupProfile? GetStreamGroupProfile(int StreamGroupId, int StreamGroupProfileId);
        Task DeleteStreamGroupProfile(StreamGroupProfile StreamGroupProfile);
        List<StreamGroupProfile> GetStreamGroupProfiles();

        Task<StreamGroupProfileDto> GetDefaultStreamGroupProfile(int StreamGroupId);
        void Update(StreamGroupProfile streamGroupProfile);
    }
}