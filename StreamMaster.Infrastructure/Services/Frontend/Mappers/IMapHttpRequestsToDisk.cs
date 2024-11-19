using Microsoft.AspNetCore.Mvc;

namespace StreamMaster.Infrastructure.Services.Frontend.Mappers
{
    public interface IMapHttpRequestsToDisk
    {
        Task<string> MapAsync(string resourceUrl);
        bool CanHandle(string resourceUrl);
        Task<IActionResult?> GetResponseAsync(string resourceUrl);
    }
}
