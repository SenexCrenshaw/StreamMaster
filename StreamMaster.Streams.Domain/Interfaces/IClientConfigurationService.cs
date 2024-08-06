using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IClientConfigurationService
    {
        IClientConfiguration NewClientConfiguration(string uniqueRequestId, SMChannelDto smChannel, int streamGroupProfileId, string clientUserAgent, string clientIPAddress, HttpResponse response, CancellationToken cancellationToken);
    }
}