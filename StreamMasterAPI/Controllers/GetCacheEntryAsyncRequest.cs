using MediatR;

using StreamMasterDomain.Enums;
using StreamMasterDomain.Repository;

namespace StreamMasterAPI.Controllers;

public class GetCacheEntryAsyncRequest : IRequest<(CacheEntity? cacheEntry, byte[]? data)>
{
    public SMFileTypes IPTVFileType { get; set; }
    public string? URL { get; set; }
}
