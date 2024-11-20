using MediatR.Pipeline;

namespace StreamMaster.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest>(ILogger<TRequest> logger) : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        if (requestName.Equals("GetM3UFileIdMaxStreamFromUrl"))
        {
            logger.LogInformation("Request: {Name} ", requestName);
        }
        else
        {
            logger.LogInformation("Request: {Name}  {@Request}", requestName, request);
        }

        return Task.CompletedTask;
    }
}