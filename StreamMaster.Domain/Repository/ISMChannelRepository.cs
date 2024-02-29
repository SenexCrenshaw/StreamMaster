namespace StreamMaster.Domain.Repository;

public interface ISMChannelRepository
{
    IQueryable<SMChannel> GetQuery(bool tracking = false);
    List<SMChannelDto> GetSMChannels();
}