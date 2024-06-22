using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Extensions;

namespace StreamMaster.Streams.Domain.Statistics;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]

public class BaseStatistics
{
    public DateTimeOffset StartTime { get; set; }
    public string ElapsedTime { get; set; } = string.Empty;
    public virtual void UpdateValues()
    {
        TimeSpan elapsedTime = new(0);
        if (StartTime != DateTimeOffset.MinValue)
        {
            elapsedTime = SMDT.UtcNow - StartTime;
        }
        ElapsedTime = $"{elapsedTime.Days} {elapsedTime.Hours:00}:{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
    }
}
