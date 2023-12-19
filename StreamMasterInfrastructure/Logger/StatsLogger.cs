using Microsoft.Extensions.Logging;

namespace StreamMasterInfrastructure.Logger;

public class StatsLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new StatsLogger();
    }

    public void Dispose()
    {
    }
}

public class StatsLogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);

        // Custom log formatting logic here
        // Remove the namespace from the message or format as needed

        Console.WriteLine($"{logLevel}: {message}");
    }
}
