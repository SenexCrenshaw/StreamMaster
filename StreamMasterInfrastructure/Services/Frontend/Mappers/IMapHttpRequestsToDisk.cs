using Microsoft.AspNetCore.Mvc;

namespace StreamMasterInfrastructure.Services.Frontend.Mappers
{
    public interface IMapHttpRequestsToDisk
    {
        Task<string> Map(string resourceUrl);
        bool CanHandle(string resourceUrl);
        Task<IActionResult> GetResponse(string resourceUrl);
    }
}
