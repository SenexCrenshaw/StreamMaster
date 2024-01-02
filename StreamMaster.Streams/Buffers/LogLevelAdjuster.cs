using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace StreamMaster.Streams.Buffers;

public class LogLevelAdjuster
{
    private readonly IOptionsMonitor<LoggerFilterOptions> _loggerFilterOptions;

    public LogLevelAdjuster(IOptionsMonitor<LoggerFilterOptions> loggerFilterOptions)
    {
        _loggerFilterOptions = loggerFilterOptions;
    }

    public void SetLogLevel(string category, LogLevel level)
    {
        LoggerFilterOptions filterOptions = _loggerFilterOptions.CurrentValue;
        filterOptions.Rules.Add(new LoggerFilterRule(null, category, level, null));
    }
}
