namespace StreamMaster.Domain.Cache
{
    public interface ISMCache<T>
    {
        Task SetBulkAsync(Dictionary<string, string> items, TimeSpan? slidingExpiration = null, bool noSave = false);
        Task SaveAsync();
        Task<TValue?> GetAsync<TValue>(string? key = null);
        Task<bool> ExistsAsync(string? key);
        Task SetAsync(string? key, string value, TimeSpan? slidingExpiration = null);
        Task SetAsync<TValue>(TValue value, TimeSpan? slidingExpiration = null);
        //Task SetAsync<TValue>(string? key, TValue value);
        Task RemoveAsync(string? key = null);
        Task<List<string>> GetExpiredKeysAsync();
        Task ClearAsync();
    }
}
