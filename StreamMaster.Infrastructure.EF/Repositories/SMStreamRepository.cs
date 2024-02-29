using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class SMStreamRepository(ILogger<SMStreamRepository> intLogger, IRepositoryContext repositoryContext, IMapper mapper) : RepositoryBase<SMStream>(repositoryContext, intLogger), ISMStreamRepository
{
    public List<SMStreamDto> GetSMStreams()
    {
        return [.. FindAll().ProjectTo<SMStreamDto>(mapper.ConfigurationProvider)];
    }

    public IQueryable<SMStream> GetQuery(bool tracking = false)
    {
        return tracking ? FindAllWithTracking() : FindAll();
    }

    public PagedResponse<SMStreamDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<SMStreamDto>(Count());
    }

    public async Task<PagedResponse<SMStreamDto>> GetPagedSMStreams(SMStreamParameters parameters, CancellationToken cancellationToken)
    {
        IQueryable<SMStream> query = GetIQueryableForEntity(parameters);
        return await query.GetPagedResponseAsync<SMStream, SMStreamDto>(parameters.PageNumber, parameters.PageSize, mapper)
                          .ConfigureAwait(false);
    }


}
