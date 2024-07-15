using System.Diagnostics;

namespace StreamMaster.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse>(ILogger<TRequest> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //if (!_settings.LogPerformance)
        //{
        //    return await next().ConfigureAwait(false);
        //}

        //_timer.Start();



        //_timer.Stop();

        //long elapsedMilliseconds = _timer.ElapsedMilliseconds;

        ////if (elapsedMilliseconds > 1)
        ////{
        //_logger.LogInformation("LogPerformance: {ElapsedMilliseconds} milliseconds", elapsedMilliseconds);
        ////}

        return await next().ConfigureAwait(false);
    }
}
