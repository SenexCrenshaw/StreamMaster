using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class SMChannelRepository(ILogger<SMChannelRepository> intLogger, IRepositoryContext repositoryContext, IMapper mapper) : RepositoryBase<SMChannel>(repositoryContext, intLogger), ISMChannelRepository
{
    public List<SMChannelDto> GetSMChannels()
    {
        return [.. FindAll().ProjectTo<SMChannelDto>(mapper.ConfigurationProvider)];
    }

    public IQueryable<SMChannel> GetQuery(bool tracking = false)
    {
        return tracking ? FindAllWithTracking() : FindAll();
    }

}
