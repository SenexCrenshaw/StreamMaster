namespace StreamMaster.Domain.Enums;

public enum JobType
{
    ProcessM3U,
    RefreshM3U,
    TimerM3U,
    ProcessEPG,
    RefreshEPG,
    TimerEPG,
    SDSync,
    Backup,
    TimerBackup
}