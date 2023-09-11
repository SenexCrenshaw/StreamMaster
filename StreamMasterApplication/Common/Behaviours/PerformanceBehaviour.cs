using System.Diagnostics;

namespace StreamMasterApplication.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;
    private readonly Stopwatch _timer;
    private readonly ISettingsService _settingsService;
    public PerformanceBehaviour(ILogger<TRequest> logger, ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _timer = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //if (!_settings.LogPerformance)
        //{
        //    return await next().ConfigureAwait(false);
        //}

        //_timer.Start();

        TResponse? response = await next().ConfigureAwait(false);

        //_timer.Stop();

        //long elapsedMilliseconds = _timer.ElapsedMilliseconds;

        ////if (elapsedMilliseconds > 1)
        ////{
        //_logger.LogInformation("LogPerformance: {ElapsedMilliseconds} milliseconds", elapsedMilliseconds);
        ////}

        return response;
    }
}
