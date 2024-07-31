using Microsoft.Extensions.DependencyInjection;

namespace StreamMaster.Application.StreamGroups;

public class StreamGroupService(IMemoryCache _memoryCache, IServiceProvider _serviceProvider) : IStreamGroupService
{
    private const string DefaultStreamGroupName = "all";
    private const string CacheKey = "DefaultStreamGroup";
    public async Task<int> GetDefaultSGIdAsync()
    {
        StreamGroup sg = await GetDefaultSGAsync();
        return sg.Id;
    }

    public async Task<StreamGroup?> GetStreamGroupFromIdAsync(int streamGroupId)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        return await repositoryWrapper.StreamGroup.FirstOrDefaultAsync(a => a.Id == streamGroupId).ConfigureAwait(false);
    }

    public async Task<StreamGroup?> GetStreamGroupFromNameAsync(string streamGroupName)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        return await repositoryWrapper.StreamGroup.FirstOrDefaultAsync(a => a.Name == streamGroupName).ConfigureAwait(false);
    }

    public async Task<StreamGroup> GetDefaultSGAsync()
    {
        if (_memoryCache.TryGetValue(CacheKey, out StreamGroup? streamGroup))
        {
            if (streamGroup != null)
            {
                return streamGroup;
            }
        }

        StreamGroup sg = await GetStreamGroupFromNameAsync(DefaultStreamGroupName).ConfigureAwait(false) ?? throw new Exception("StreamGroup 'All' not found");

        _memoryCache.Set(CacheKey, sg, new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove
        });

        return sg;
    }
}
