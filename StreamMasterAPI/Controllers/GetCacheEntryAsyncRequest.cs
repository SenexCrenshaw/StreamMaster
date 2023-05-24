using MediatR;

using StreamMasterDomain.Entities;
using StreamMasterDomain.Enums;

namespace StreamMasterAPI.Controllers;

public class GetCacheEntryAsyncRequest : IRequest<(CacheEntity? cacheEntry, byte[]? data)>
{
    public SMFileTypes IPTVFileType { get; set; }
    public string? URL { get; set; }
}
