using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Extensions;

namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SMTask(int Id)
{
    public bool IsRunning { get; set; }
    public string Command { get; set; } = string.Empty;
    public DateTime QueueTS { get; set; } = SMDT.UtcNow;
    public DateTime StartTS { get; set; } = DateTime.MinValue;
    public DateTime StopTS { get; set; } = DateTime.MinValue;
    //public string ElapsedTS = "0";
    public string Status { get; set; } = string.Empty;
    //public string GetElapsedTimeFormatted()
    //{
    //    if (StartTS == DateTime.MinValue) { return string.Empty; }

    //    DateTime stop = StopTS == DateTime.MinValue ? SMDT.UtcNow : StopTS;
    //    TimeSpan elapsedTime = stop - StartTS;
    //    return $"{elapsedTime.Days} {elapsedTime.Hours:00}:{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
    //}
    public void SetIsRunning(bool IsRunning)
    {
        this.IsRunning = IsRunning;
    }
}