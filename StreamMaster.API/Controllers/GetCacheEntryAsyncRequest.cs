using MediatR;

using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Models;

namespace StreamMasterAPI.Controllers;

public class GetCacheEntryAsyncRequest : IRequest<(CacheEntity? cacheEntry, byte[]? data)>
{
    public SMFileTypes IPTVFileType { get; set; }
    public string? URL { get; set; }
}
