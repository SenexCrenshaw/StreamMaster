using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams.Handlers;

public class ClientConfigurationService(ILoggerFactory loggerFactory) : IClientConfigurationService
{
    public IClientConfiguration NewClientConfiguration(string uniqueRequestId,
        SMChannelDto smChannel,
        int streamGroupId,
         int streamGroupProfileId,
        string clientUserAgent,
        string clientIPAddress,
        HttpResponse response,
        CancellationToken cancellationToken)
    {
        ClientConfiguration config = new(uniqueRequestId, smChannel, streamGroupId, streamGroupProfileId, clientUserAgent, clientIPAddress, response, loggerFactory, cancellationToken);

        config.ClientStream ??= new ClientReadStream(loggerFactory, config.UniqueRequestId);
        return config;
    }
}