using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IVideoService
    {
        Task<StreamResult> AddClientToChannelAsync(HttpContext HttpContext, int? streamGroupId, int? streamGroupProfileId, int? smChannelId, CancellationToken cancellationToken);
    }
}