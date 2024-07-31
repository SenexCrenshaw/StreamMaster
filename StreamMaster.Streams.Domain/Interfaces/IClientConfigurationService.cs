using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IClientConfigurationService
    {
        IClientConfiguration NewClientConfiguration(string uniqueRequestId, SMChannelDto smChannel, int streamGroupId, int streamGroupProfileId, string clientUserAgent, string clientIPAddress, HttpResponse response, CancellationToken cancellationToken);
    }
}