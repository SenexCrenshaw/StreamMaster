using Microsoft.Extensions.Diagnostics.HealthChecks;

using StreamMaster.Streams.Streams;

namespace StreamMaster.Streams.Handler;

public class StreamHandlerHealthCheck : IHealthCheck
{
    private readonly StreamHandler _streamHandler;

    public StreamHandlerHealthCheck(StreamHandler streamHandler)
    {
        _streamHandler = streamHandler;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        bool isHealthy = _streamHandler.IsHealthy(); // Implement this method to check your stream handler's health

        return isHealthy
            ? Task.FromResult(HealthCheckResult.Healthy("StreamHandler is healthy."))
            : Task.FromResult(HealthCheckResult.Unhealthy("StreamHandler is unhealthy."));
    }


}
