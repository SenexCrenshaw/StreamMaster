namespace StreamMaster.Domain.Cache
{
    public interface IHybridCache<T>
    {
        Task<bool> ExistsAsync(string key);
        Task<string?> GetAsync(string key);
        Task SetAsync(string key, string value, TimeSpan? slidingExpiration = null);
        Task RemoveAsync(string key);
        Task<List<string>> GetExpiredKeysAsync();
        Task ClearAsync();
    }
}
